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

namespace GuiStack.Models
{
    public class SNSTopicModel
    {
        public SNSTopicInfo Topic { get; set; }
        public IEnumerable<SNSSubscription> Subscriptions { get; set; }

        public SNSTopicModel(SNSTopicInfo topicInfo, IEnumerable<SNSSubscription> subscriptions)
        {
            Topic = topicInfo;
            Subscriptions = subscriptions;
        }
    }
}
