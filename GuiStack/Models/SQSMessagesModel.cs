using System;
using System.Collections.Generic;

namespace GuiStack.Models
{
    public class SQSMessagesModel
    {
        public IEnumerable<SQSMessage> Messages { get; set; }
        public string Prefix { get; set; }
    }
}
