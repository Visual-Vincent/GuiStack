using System;
using Amazon.S3;

namespace GuiStack.Services
{
    public interface IS3UrlBuilder
    {
        string GetHttpUrl(AmazonS3Client client, string bucketName, string objectName);
        string GetS3Uri(AmazonS3Client client, string bucketName, string objectName);
    }

    public class S3UrlBuilder : IS3UrlBuilder
    {
        private const string S3UriFormat = "s3://{0}/{1}";

        public string GetHttpUrl(AmazonS3Client client, string bucketName, string objectName)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            bucketName = Uri.EscapeDataString(bucketName);
            objectName = Uri.EscapeDataString(objectName);

            Uri url = new Uri(client.Config.DetermineServiceURL());

            if(EnvironmentVariables.S3ForcePathStyle)
                return $"{url.Scheme}://{url.Authority}/{bucketName}/{objectName}";

            return $"{url.Scheme}://{bucketName}.{url.Authority}/{objectName}";
        }

        public string GetS3Uri(AmazonS3Client client, string bucketName, string objectName)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            return string.Format(S3UriFormat, Uri.EscapeDataString(bucketName), Uri.EscapeDataString(objectName));
        }
    }
}
