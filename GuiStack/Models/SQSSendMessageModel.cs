using System;

namespace GuiStack.Models
{
    public class SQSSendMessageModel
    {
        public bool IsProtobuf { get; set; }
        public bool Base64Encode { get; set; }
        public string Body { get; set; }
    }
}
