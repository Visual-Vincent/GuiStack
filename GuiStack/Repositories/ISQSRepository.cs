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
        Task CreateQueueAsync(SQSCreateQueueModel model);
        Task<IEnumerable<SQSQueue>> GetQueuesAsync();
        Task<SQSQueueInfo> GetQueueAttributesAsync(string queueUrl);
        Task<string> GetQueueUrlAsync(string queueName);
        Task DeleteMessageAsync(string queueUrl, string receiptHandle);
        Task DeleteQueueAsync(string queueUrl);
        Task<IEnumerable<SQSMessage>> ReceiveMessagesAsync(string queueUrl, int maxAmount, int waitTimeSeconds = 0);
        Task<string> SendMessageAsync(string queueUrl, string messageBody);
    }

    public class SQSRepository : ISQSRepository
    {
        private SQSAuthenticator authenticator = new SQSAuthenticator();

        public async Task CreateQueueAsync(SQSCreateQueueModel model)
        {
            using var sqs = authenticator.Authenticate();
            string queueName = model.QueueName;

            if(model.IsFifo)
                if(queueName.EndsWith(".fifo", StringComparison.OrdinalIgnoreCase))
                    queueName = $"{queueName.Remove(queueName.Length - 5, 5)}.fifo"; // Ensure lowercase ".fifo"
                else
                    queueName = $"{queueName}.fifo";

            var response = await sqs.CreateQueueAsync(new CreateQueueRequest() {
                QueueName = queueName,
                Attributes = new Dictionary<string, string>() {
                    { "FifoQueue", model.IsFifo.ToString().ToLower() }
                }
            });

            response.ThrowIfUnsuccessful("SQS");
        }

        public async Task<IEnumerable<SQSQueue>> GetQueuesAsync()
        {
            using var sqs = authenticator.Authenticate();
            var response = await sqs.ListQueuesAsync(new ListQueuesRequest());

            response.ThrowIfUnsuccessful("SQS");

            return response.QueueUrls.Select(url => new SQSQueue(url));
        }

        public async Task<SQSQueueInfo> GetQueueAttributesAsync(string queueUrl)
        {
            if(string.IsNullOrWhiteSpace(queueUrl))
                throw new ArgumentNullException(nameof(queueUrl));

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
                FifoQueue = response.FifoQueue,
                LastModifiedTimestamp = response.LastModifiedTimestamp,
                MaximumMessageSize = response.MaximumMessageSize,
                MessageRetentionPeriod = response.MessageRetentionPeriod,
                QueueARN = response.QueueARN,
                QueueURL = queueUrl,
                ReceiveMessageWaitTimeSeconds = ReceiveMessageWaitTimeSeconds,
                VisibilityTimeout = response.VisibilityTimeout,
            };
        }

        public async Task<string> GetQueueUrlAsync(string queueName)
        {
            if(string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentNullException(nameof(queueName));

            using var sqs = authenticator.Authenticate();
            var response = await sqs.GetQueueUrlAsync(queueName);

            response.ThrowIfUnsuccessful("SQS");

            return response.QueueUrl;
        }

        public async Task DeleteMessageAsync(string queueUrl, string receiptHandle)
        {
            if(string.IsNullOrWhiteSpace(queueUrl))
                throw new ArgumentNullException(nameof(queueUrl));

            if(string.IsNullOrWhiteSpace(receiptHandle))
                throw new ArgumentNullException(nameof(receiptHandle));

            using var sqs = authenticator.Authenticate();
            await sqs.DeleteMessageAsync(queueUrl, receiptHandle);
        }

        public async Task DeleteQueueAsync(string queueUrl)
        {
            if(string.IsNullOrWhiteSpace(queueUrl))
                throw new ArgumentNullException(nameof(queueUrl));

            using var sqs = authenticator.Authenticate();
            await sqs.DeleteQueueAsync(queueUrl);
        }

        public async Task<IEnumerable<SQSMessage>> ReceiveMessagesAsync(string queueUrl, int maxAmount, int waitTimeSeconds = 0)
        {
            if(string.IsNullOrWhiteSpace(queueUrl))
                throw new ArgumentNullException(nameof(queueUrl));

            if(maxAmount < 1 || maxAmount > 10)
                throw new ArgumentOutOfRangeException(nameof(maxAmount), $"{nameof(maxAmount)} must be between 1-10");

            if(waitTimeSeconds < 0 || waitTimeSeconds > 20)
                throw new ArgumentOutOfRangeException(nameof(waitTimeSeconds), $"{nameof(waitTimeSeconds)} must be between 0-20");

            using var sqs = authenticator.Authenticate();

            var response = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest() {
                QueueUrl = queueUrl,
                AttributeNames = new List<string>() { "SentTimestamp", "MessageGroupId", "SequenceNumber" },
                MaxNumberOfMessages = maxAmount,
                WaitTimeSeconds = waitTimeSeconds,
            });

            response.ThrowIfUnsuccessful("SQS");

            return response.Messages.Select(m => new SQSMessage() {
                Attributes = m.MessageAttributes.ToStringAttributes(),
                Body = m.Body,
                MessageId = m.MessageId,
                ReceiptHandle = m.ReceiptHandle,
                MessageGroupId = m.Attributes.GetValueOrDefault("MessageGroupId"),
                SequenceNumber = m.Attributes.GetValueOrDefault("SequenceNumber"),
                SentTimestamp = m.Attributes.ContainsKey("SentTimestamp")
                    ? DateTime.SpecifyKind(
                        DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(m.Attributes["SentTimestamp"])).DateTime,
                        DateTimeKind.Utc
                      ).ToLocalTime()
                    : (DateTime?)null
            });
        }

        public async Task<string> SendMessageAsync(string queueUrl, string messageBody)
        {
            using var sqs = authenticator.Authenticate();
            var response = await sqs.SendMessageAsync(new SendMessageRequest(queueUrl, messageBody));

            response.ThrowIfUnsuccessful("SQS");

            return response.MessageId;
        }
    }
}
