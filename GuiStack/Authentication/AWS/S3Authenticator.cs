using System;
using Amazon.Runtime;
using Amazon.S3;

namespace GuiStack.Authentication.AWS
{
    public class S3Authenticator : Authenticator<AWSCredentials, AmazonS3Client>
    {
        public override AmazonS3Client Authenticate(AWSCredentials credentials)
        {
            var config = new AmazonS3Config() {
                MaxErrorRetry = 1
            };

            string endpointUrl = Environment.GetEnvironmentVariable("AWS_S3_ENDPOINT_URL");

            if(!string.IsNullOrWhiteSpace(endpointUrl))
                config.ServiceURL = endpointUrl;

            return new AmazonS3Client(credentials, config);
        }

        public override AWSCredentials GetCredentials()
        {
            return new BasicAWSCredentials(
                Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
                Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
            );
        }
    }
}
