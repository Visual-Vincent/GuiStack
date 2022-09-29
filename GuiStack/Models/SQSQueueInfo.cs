/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;

namespace GuiStack.Models
{
    public class SQSQueueInfo
    {
        public int ApproximateNumberOfMessages { get; set; }
        public int ApproximateNumberOfMessagesDelayed { get; set; }
        public int ApproximateNumberOfMessagesNotVisible { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int DelaySeconds { get; set; }
        public bool FifoQueue { get; set; }
        public DateTime LastModifiedTimestamp { get; set; }
        public int MaximumMessageSize { get; set; }
        public int MessageRetentionPeriod { get; set; }
        public string QueueARN { get; set; }
        public string QueueURL { get; set; }
        public int ReceiveMessageWaitTimeSeconds { get; set; }
        public int VisibilityTimeout { get; set; }
    }
}
