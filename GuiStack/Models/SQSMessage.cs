using System;
using System.Collections.Generic;

namespace GuiStack.Models
{
    public class SQSMessage
    {
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public string Body { get; set; }
        public string MessageId { get; set; }
        public string MessageGroupId { get; set; }
        public string ReceiptHandle { get; set; }
        public DateTime? SentTimestamp { get; set; }
        public string SequenceNumber { get; set; }
    }
}
