using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GuiStack.Authentication.AWS;
using GuiStack.Extensions;
using GuiStack.Models;
using GuiStack.Services;

namespace GuiStack.Repositories
{
    public interface IS3Repository
    {
        Task CreateBucketAsync(string bucketName);
        Task DeleteObjectAsync(string bucketName, string objectName);
        Task<IEnumerable<S3Bucket>> GetBucketsAsync();
        Task<IEnumerable<S3Object>> GetObjectsAsync(string bucketName);
        Task<Amazon.S3.Model.GetObjectResponse> GetObjectAsync(string bucketName, string objectName);
        Task RenameObjectAsync(string bucketName, string objectName, string newName);
    }

    public class S3Repository : IS3Repository
    {
        // https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-keys.html
        private static readonly Regex InvalidObjectCharsRegex = new Regex("[^A-Z0-9!._*'()-]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private S3Authenticator authenticator = new S3Authenticator();
        private IS3UrlBuilder urlBuilder;

        public S3Repository(IS3UrlBuilder urlBuilder)
        {
            this.urlBuilder = urlBuilder;
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            using var s3 = authenticator.Authenticate();
            var response = await s3.PutBucketAsync(new Amazon.S3.Model.PutBucketRequest() {
                BucketName = bucketName,
                UseClientRegion = true
            });

            response.ThrowIfUnsuccessful("S3");
        }

        public async Task DeleteObjectAsync(string bucketName, string objectName)
        {
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
    }
}
