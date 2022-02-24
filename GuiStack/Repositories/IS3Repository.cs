using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GuiStack.Authentication.AWS;
using GuiStack.Models;

namespace GuiStack.Repositories
{
    public interface IS3Repository
    {
        Task<IEnumerable<S3Bucket>> GetBucketsAsync();
        Task<IEnumerable<S3Object>> GetObjectsAsync(string bucketName);
        Task<Amazon.S3.Model.GetObjectResponse> GetObjectAsync(string bucketName, string objectName);
    }

    public class S3Repository : IS3Repository
    {
        private S3Authenticator authenticator = new S3Authenticator();

        public async Task<IEnumerable<S3Bucket>> GetBucketsAsync()
        {
            using var s3 = authenticator.Authenticate();
            var response = await s3.ListBucketsAsync();

            if(response.HttpStatusCode != HttpStatusCode.OK)
                throw new WebException($"Amazon S3 returned status code {(int)response.HttpStatusCode}");

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

            if(response.HttpStatusCode != HttpStatusCode.OK)
                throw new WebException($"Amazon S3 returned status code {(int)response.HttpStatusCode}");

            return response.S3Objects.Select(obj => new S3Object() {
                Name = obj.Key,
                Size = obj.Size,
                LastModified = obj.LastModified
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
    }
}
