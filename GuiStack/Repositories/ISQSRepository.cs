using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using GuiStack.Authentication.AWS;
using GuiStack.Extensions;
using GuiStack.Models;

namespace GuiStack.Repositories
{
    public interface ISQSRepository
    {
        Task<IEnumerable<SQSQueue>> GetQueuesAsync();
        Task<SQSQueueInfo> GetQueueAttributesAsync(string queueUrl);
        Task<string> GetQueueUrlAsync(string queueName);
    }

    public class SQSRepository : ISQSRepository
    {
        private SQSAuthenticator authenticator = new SQSAuthenticator();

        public async Task<IEnumerable<SQSQueue>> GetQueuesAsync()
        {
            using var sqs = authenticator.Authenticate();
            var response = await sqs.ListQueuesAsync(new ListQueuesRequest());

            response.ThrowIfUnsuccessful("SQS");

            return response.QueueUrls.Select(url => new SQSQueue(url));
        }

        public async Task<SQSQueueInfo> GetQueueAttributesAsync(string queueUrl)
        {
            using var sqs = authenticator.Authenticate();
            var response = await sqs.GetQueueAttributesAsync(queueUrl, new List<string>() {
                "ApproximateNumberOfMessages", "ApproximateNumberOfMessagesDelayed", "ApproximateNumberOfMessagesNotVisible",
                "CreatedTimestamp", "DelaySeconds", "LastModifiedTimestamp", "MaximumMessageSize", "MessageRetentionPeriod",
                "QueueArn", "ReceiveMessageWaitTimeSeconds", "VisibilityTimeout", "FifoQueue"
            });

            response.ThrowIfUnsuccessful("SQS");

            response.Attributes.TryGetValue("ReceiveMessageWaitTimeSeconds", out string ReceiveMessageWaitTimeSecondsStr);

            if(!int.TryParse(ReceiveMessageWaitTimeSecondsStr, out var ReceiveMessageWaitTimeSeconds))
                ReceiveMessageWaitTimeSeconds = -1;

            return new SQSQueueInfo() {
                ApproximateNumberOfMessages = response.ApproximateNumberOfMessages,
                ApproximateNumberOfMessagesDelayed = response.ApproximateNumberOfMessagesDelayed,
                ApproximateNumberOfMessagesNotVisible = response.ApproximateNumberOfMessagesNotVisible,
                CreatedTimestamp = response.CreatedTimestamp,
                DelaySeconds = response.DelaySeconds,
                LastModifiedTimestamp = response.LastModifiedTimestamp,
                MaximumMessageSize = response.MaximumMessageSize,
                MessageRetentionPeriod = response.MessageRetentionPeriod,
                QueueARN = response.QueueARN,
                ReceiveMessageWaitTimeSeconds = ReceiveMessageWaitTimeSeconds,
                VisibilityTimeout = response.VisibilityTimeout,
                FifoQueue = response.FifoQueue,
            };
        }

        public async Task<string> GetQueueUrlAsync(string queueName)
        {
            using var sqs = authenticator.Authenticate();
            var response = await sqs.GetQueueUrlAsync(queueName);

            response.ThrowIfUnsuccessful("SQS");

            return response.QueueUrl;
        }
    }
}
