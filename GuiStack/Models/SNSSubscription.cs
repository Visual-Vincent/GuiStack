/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon;

namespace GuiStack.Models
{
    public class SNSSubscription
    {
        public Arn Arn { get; set; }
        public Arn TopicARN { get; set; }
        public string Protocol { get; set; }
        public string Endpoint { get; set; }
        public string Owner { get; set; }
        public bool RawMessageDelivery { get; set; }

        public SNSSubscription()
        {
        }

        public SNSSubscription(string arn, string topicArn, string protocol, string endpoint, string owner, bool rawMessageDelivery)
        {
            Arn = Arn.Parse(arn);
            TopicARN = Arn.Parse(topicArn);
            Protocol = protocol;
            Endpoint = endpoint;
            Owner = owner;
            RawMessageDelivery = rawMessageDelivery;
        }
    }
}