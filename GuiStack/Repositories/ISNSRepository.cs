/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2023
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;
using GuiStack.Authentication.AWS;
using GuiStack.Extensions;
using GuiStack.Models;

namespace GuiStack.Repositories
{
    public interface ISNSRepository
    {
        Task CreateTopicAsync(SNSCreateTopicModel model);
        Task<IEnumerable<SNSTopic>> GetTopicsAsync();
        Task DeleteTopicAsync(string topicArn);
    }

    public class SNSRepository : ISNSRepository
    {
        private SNSAuthenticator authenticator = new SNSAuthenticator();

        public async Task CreateTopicAsync(SNSCreateTopicModel model)
        {
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
                    { "FifoTopic", model.IsFifo.ToString().ToLower() }
                }
            });

            response.ThrowIfUnsuccessful("SNS");
        }

        public async Task<IEnumerable<SNSTopic>> GetTopicsAsync()
        {
            using var sns = authenticator.Authenticate();
            var response = await sns.ListTopicsAsync(new ListTopicsRequest());
            
            response.ThrowIfUnsuccessful("SNS");

            return response.Topics.Select(topic => new SNSTopic(topic.TopicArn));
        }

        public async Task DeleteTopicAsync(string topicArn)
        {
            if(string.IsNullOrWhiteSpace(topicArn))
                throw new ArgumentNullException(nameof(topicArn));

            using var sns = authenticator.Authenticate();
            var response = await sns.DeleteTopicAsync(topicArn);

            response.ThrowIfUnsuccessful("SNS");
        }
    }
}
