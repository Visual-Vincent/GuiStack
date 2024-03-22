/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using GuiStack.Authentication.AWS;
using GuiStack.Extensions;
using GuiStack.Models;
using GuiStack.Services;

namespace GuiStack.Repositories
{
    public interface IS3Repository
    {
        Task CreateBucketAsync(string bucketName);
        Task DeleteBucketAsync(string bucketName);
        Task DeleteObjectAsync(string bucketName, string objectName);
        Task<IEnumerable<S3Bucket>> GetBucketsAsync();
        Task<IEnumerable<S3Object>> GetObjectsAsync(string bucketName);
        Task<Amazon.S3.Model.GetObjectResponse> GetObjectAsync(string bucketName, string objectName);
        Task RenameObjectAsync(string bucketName, string objectName, string newName);
        Task UploadFile(string bucketName, string filename, Stream stream);
    }

    public class S3Repository : IS3Repository
    {
        // https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-keys.html
        private static readonly Regex InvalidObjectCharsRegex = new Regex("[^A-Z0-9!._*'()/-]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private S3Authenticator authenticator = new S3Authenticator();
        private IS3UrlBuilder urlBuilder;

        public S3Repository(IS3UrlBuilder urlBuilder)
        {
            this.urlBuilder = urlBuilder;
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            if(bucketName == null)
                throw new ArgumentNullException(nameof(bucketName));

            using var s3 = authenticator.Authenticate();
            var response = await s3.PutBucketAsync(new Amazon.S3.Model.PutBucketRequest() {
                BucketName = bucketName,
                BucketRegionName = AWSConfigs.AWSRegion,
                UseClientRegion = true
            });

            response.ThrowIfUnsuccessful("S3");
        }

        public async Task DeleteBucketAsync(string bucketName)
        {
            if(bucketName == null)
                throw new ArgumentNullException(nameof(bucketName));

            using var s3 = authenticator.Authenticate();
            var response = await s3.DeleteBucketAsync(bucketName);

            response.ThrowIfUnsuccessful("S3");
        }

        public async Task DeleteObjectAsync(string bucketName, string objectName)
        {
            if(bucketName == null)
                throw new ArgumentNullException(nameof(bucketName));

            if(objectName == null)
                throw new ArgumentNullException(nameof(objectName));

            using var s3 = authenticator.Authenticate();
            var response = await s3.DeleteObjectAsync(bucketName, objectName);

            response.ThrowIfUnsuccessful("S3");
        }

        public async Task<IEnumerable<S3Bucket>> GetBucketsAsync()
        {
            using var s3 = authenticator.Authenticate();
            var response = await s3.ListBucketsAsync();
            
            response.ThrowIfUnsuccessful("S3");

            return response.Buckets.Select(b => new S3Bucket() {
                Name = b.BucketName,
                CreationDate = b.CreationDate
            });
        }

        public async Task<IEnumerable<S3Object>> GetObjectsAsync(string bucketName)
        {
            if(bucketName == null)
                throw new ArgumentNullException(nameof(bucketName));

            using var s3 = authenticator.Authenticate();
            var response = await s3.ListObjectsV2Async(new Amazon.S3.Model.ListObjectsV2Request() {
                BucketName = bucketName
            });

            response.ThrowIfUnsuccessful("S3");

            return response.S3Objects.Select(obj => new S3Object() {
                Name = obj.Key,
                Size = obj.Size,
                LastModified = obj.LastModified,
                S3Uri = urlBuilder.GetS3Uri(s3, obj.BucketName, obj.Key),
                Url = urlBuilder.GetHttpUrl(s3, obj.BucketName, obj.Key)
            });
        }

        public async Task<Amazon.S3.Model.GetObjectResponse> GetObjectAsync(string bucketName, string objectName)
        {
            if(bucketName == null)
                throw new ArgumentNullException(nameof(bucketName));

            if(objectName == null)
                throw new ArgumentNullException(nameof(objectName));

            using var s3 = authenticator.Authenticate();
            var response = await s3.GetObjectAsync(bucketName, objectName);
            return response;
        }

        public async Task RenameObjectAsync(string bucketName, string objectName, string newName)
        {
            if(bucketName == null)
                throw new ArgumentNullException(nameof(bucketName));

            if(objectName == null)
                throw new ArgumentNullException(nameof(objectName));

            if(newName == null)
                throw new ArgumentNullException(nameof(newName));

            newName = InvalidObjectCharsRegex.Replace(newName, "_");

            if(objectName == newName)
                return;

            using var s3 = authenticator.Authenticate();

            var copyResponse = await s3.CopyObjectAsync(bucketName, objectName, bucketName, newName);
            copyResponse.ThrowIfUnsuccessful("S3 copy");

            var deleteResponse = await s3.DeleteObjectAsync(bucketName, objectName);
            deleteResponse.ThrowIfUnsuccessful("S3 delete");
        }

        public async Task UploadFile(string bucketName, string filename, Stream stream)
        {
            if(bucketName == null)
                throw new ArgumentNullException(nameof(bucketName));

            if(filename == null)
                throw new ArgumentNullException(nameof(filename));

            if(stream == null)
                throw new ArgumentNullException(nameof(stream));

            if(!stream.CanRead)
                throw new ArgumentException("File upload stream must be readable", nameof(stream));

            long fileSize;

            try
            {
                fileSize = stream.Length;
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Failed to get the total upload file size", ex);
            }

            using var s3 = authenticator.Authenticate();

            var initiateUploadResponse = await s3.InitiateMultipartUploadAsync(new Amazon.S3.Model.InitiateMultipartUploadRequest() {
                BucketName = bucketName,
                Key = filename
            });

            initiateUploadResponse.ThrowIfUnsuccessful("S3");

            // 5 MB is the minimum allowed chunk size for multipart uploads
            const int ChunkSize = 5 * 1024 * 1024;

            long bytesWritten = 0;
            int partNumber = 0;

            string uploadId = initiateUploadResponse.UploadId;

            try
            {
                var uploadResponses = new List<Amazon.S3.Model.UploadPartResponse>();

                while(bytesWritten < fileSize)
                {
                    long remaining = fileSize - bytesWritten;
                    bool isLastPart = remaining <= ChunkSize;
                    int partSize = isLastPart ? (int)remaining : ChunkSize;

                    partNumber++;

                    var response = await s3.UploadPartAsync(new Amazon.S3.Model.UploadPartRequest() {
                        UploadId = uploadId,
                        BucketName = bucketName,
                        Key = filename,
                        InputStream = stream,
                        PartSize = partSize,
                        PartNumber = partNumber,
                        IsLastPart = isLastPart,
                    });

                    response.ThrowIfUnsuccessful("S3");

                    bytesWritten += partSize;
                    uploadResponses.Add(response);

                    if(isLastPart)
                        break;
                }

                var completeUploadRequest = new Amazon.S3.Model.CompleteMultipartUploadRequest() {
                    UploadId = uploadId,
                    BucketName = bucketName,
                    Key = filename,
                };

                completeUploadRequest.AddPartETags(uploadResponses);

                (await s3.CompleteMultipartUploadAsync(completeUploadRequest)).ThrowIfUnsuccessful("S3");
            }
            catch
            {
                // Deliberately ignore AWS API response, since we don't care if an error occurred here
                await s3.AbortMultipartUploadAsync(bucketName, filename, uploadId);

                throw;
            }
        }
    }
}
