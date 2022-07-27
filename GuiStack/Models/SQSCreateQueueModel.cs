using System;

namespace GuiStack.Models
{
    public class SQSCreateQueueModel
    {
        public string QueueName { get; set; }
        public bool IsFifo { get; set; }
    }
}
