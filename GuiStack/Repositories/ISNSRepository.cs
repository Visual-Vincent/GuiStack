/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService.Model;
using GuiStack.Authentication.AWS;
using GuiStack.Extensions;
using GuiStack.Models;

namespace GuiStack.Repositories
{
    public interface ISNSRepository
    {
        Task CreateTopicAsync(SNSCreateTopicModel model);
        Task CreateTopicSubscriptionAsync(SNSCreateSubscriptionModel model);
        Task<IEnumerable<SNSTopic>> GetTopicsAsync();
        Task<SNSTopicInfo> GetTopicAttributesAsync(string topicArn);
        Task<IEnumerable<SNSSubscription>> GetTopicSubscriptionsAsync(string topicArn);
        Task DeleteTopicAsync(string topicArn);
        Task DeleteSubscriptionAsync(string subscriptionArn);
        Task<string> SendMessageAsync(string topicArn, string messageBody);
    }

    public class SNSRepository : ISNSRepository
    {
        private SNSAuthenticator authenticator = new SNSAuthenticator();
        private SQSAuthenticator sqsAuthenticator = new SQSAuthenticator();

        public async Task CreateTopicAsync(SNSCreateTopicModel model)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            using var sns = authenticator.Authenticate();
            string topicName = model.TopicName;
            
            if(model.IsFifo)
                if(topicName.EndsWith(".fifo", StringComparison.OrdinalIgnoreCase))
                    topicName = $"{topicName.Remove(topicName.Length - 5, 5)}.fifo"; // Ensure lowercase ".fifo"
                else
                    topicName = $"{topicName}.fifo";

            var response = await sns.CreateTopicAsync(new CreateTopicRequest() {
                Name = topicName,
                Attributes = new Dictionary<string, string>() {
                    { "FifoTopic", model.IsFifo.ToString().ToLower() },
                    { "ContentBasedDeduplication", model.IsFifo ? model.ContentBasedDeduplication.ToString().ToLower() : "false" }
                }
            });

            response.ThrowIfUnsuccessful("SNS");
        }

        public async Task CreateTopicSubscriptionAsync(SNSCreateSubscriptionModel model)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(!model.Protocol.Equals("sqs", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Protocol '{model.Protocol}' is not supported. Supported protocols are: sqs", nameof(model));

            using var sns = authenticator.Authenticate();
            using var sqs = sqsAuthenticator.Authenticate();

            var subscriptionArn = await sns.SubscribeQueueAsync(model.TopicArn, sqs, model.Endpoint);
            var response = await sns.SetSubscriptionAttributesAsync(subscriptionArn, "RawMessageDelivery", model.RawMessageDelivery.ToString().ToLower());

            response.ThrowIfUnsuccessful("SNS");
        }

        public async Task<IEnumerable<SNSTopic>> GetTopicsAsync()
        {
            using var sns = authenticator.Authenticate();
            var response = await sns.ListTopicsAsync(new ListTopicsRequest());
            
            response.ThrowIfUnsuccessful("SNS");

            return response.Topics.Select(topic => new SNSTopic(topic.TopicArn));
        }

        public async Task<SNSTopicInfo> GetTopicAttributesAsync(string topicName)
        {
            using var sns = authenticator.Authenticate();
            var topic = await sns.FindTopicAsync(topicName);
            
            if(topic == null)
                throw new KeyNotFoundException($"Topic with name '{topicName}' was not found");

            var response = await sns.GetTopicAttributesAsync(topic.TopicArn);

            response.ThrowIfUnsuccessful("SNS");

            var subscriptionsConfirmed    = response.Attributes.GetValueOrDefault("SubscriptionsConfirmed");
            var subscriptionsDeleted      = response.Attributes.GetValueOrDefault("SubscriptionsDeleted");
            var subscriptionsPending      = response.Attributes.GetValueOrDefault("SubscriptionsPending");
            var fifoTopic                 = response.Attributes.GetValueOrDefault("FifoTopic") ?? "";
            var contentBasedDeduplication = response.Attributes.GetValueOrDefault("ContentBasedDeduplication") ?? "";

            return new SNSTopicInfo() {
                TopicARN                  = Arn.Parse(topic.TopicArn),
                SubscriptionsConfirmed    = int.TryParse(subscriptionsConfirmed ?? "", out var i) ? i : 0,
                SubscriptionsDeleted      = int.TryParse(subscriptionsDeleted   ?? "", out i)     ? i : 0,
                SubscriptionsPending      = int.TryParse(subscriptionsPending   ?? "", out i)     ? i : 0,
                FifoTopic                 = fifoTopic.Equals("true", StringComparison.OrdinalIgnoreCase),
                ContentBasedDeduplication = contentBasedDeduplication.Equals("true", StringComparison.OrdinalIgnoreCase)
            };
        }

        public async Task<IEnumerable<SNSSubscription>> GetTopicSubscriptionsAsync(string topicArn)
        {
            using var sns = authenticator.Authenticate();
            var response = await sns.ListSubscriptionsByTopicAsync(new ListSubscriptionsByTopicRequest(topicArn));

            response.ThrowIfUnsuccessful("SNS");

            var subscriptions = new List<SNSSubscription>();

            foreach(var subscription in response.Subscriptions)
            {
                var attributesResponse = await sns.GetSubscriptionAttributesAsync(subscription.SubscriptionArn);
                attributesResponse.ThrowIfUnsuccessful("SNS");

                var rawMessageDelivery = attributesResponse.Attributes.GetValueOrDefault("RawMessageDelivery") ?? "";

                subscriptions.Add(new SNSSubscription(
                    subscription.SubscriptionArn,
                    subscription.TopicArn,
                    subscription.Protocol,
                    subscription.Endpoint,
                    subscription.Owner,
                    rawMessageDelivery.Equals("true", StringComparison.OrdinalIgnoreCase)
                ));
            }

            return subscriptions;
        }

        public async Task DeleteTopicAsync(string topicArn)
        {
            if(string.IsNullOrWhiteSpace(topicArn))
                throw new ArgumentNullException(nameof(topicArn));

            using var sns = authenticator.Authenticate();
            var response = await sns.DeleteTopicAsync(topicArn);

            response.ThrowIfUnsuccessful("SNS");
        }

        public async Task DeleteSubscriptionAsync(string subscriptionArn)
        {
            if(string.IsNullOrWhiteSpace(subscriptionArn))
                throw new ArgumentNullException(nameof(subscriptionArn));

            using var sns = authenticator.Authenticate();
            var response = await sns.UnsubscribeAsync(subscriptionArn);

            response.ThrowIfUnsuccessful("SNS");
        }

        public async Task<string> SendMessageAsync(string topicArn, string messageBody)
        {
            if(string.IsNullOrWhiteSpace(topicArn))
                throw new ArgumentNullException(nameof(topicArn));

            if(string.IsNullOrWhiteSpace(messageBody))
                throw new ArgumentNullException(nameof(messageBody));

            using var sns = authenticator.Authenticate();
            var response = await sns.PublishAsync(new PublishRequest(topicArn, messageBody));

            response.ThrowIfUnsuccessful("SNS");

            return response.MessageId;
        }
    }
}
