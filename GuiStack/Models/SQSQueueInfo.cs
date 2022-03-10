﻿using System;

namespace GuiStack.Models
{
    public class SQSQueueInfo
    {
        public int ApproximateNumberOfMessages { get; set; }
        public int ApproximateNumberOfMessagesDelayed { get; set; }
        public int ApproximateNumberOfMessagesNotVisible { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public int DelaySeconds { get; set; }
        public DateTime LastModifiedTimestamp { get; set; }
        public int MaximumMessageSize { get; set; }
        public int MessageRetentionPeriod { get; set; }
        public string QueueARN { get; set; }
        public int ReceiveMessageWaitTimeSeconds { get; set; }
        public int VisibilityTimeout { get; set; }
        public bool FifoQueue { get; set; }
    }
}