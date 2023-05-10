/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2023
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon;

namespace GuiStack.Models
{
    public class SNSTopicInfo
    {
        public Arn TopicARN { get; set; }
        public int SubscriptionsConfirmed { get; set; }
        public int SubscriptionsDeleted { get; set; }
        public int SubscriptionsPending { get; set; }
        public bool FifoTopic { get; set; }
        public bool ContentBasedDeduplication { get; set; }
    }
}
