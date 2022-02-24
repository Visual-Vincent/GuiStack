using System;

namespace GuiStack.Models
{
    public class S3Object
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string S3Uri { get; set; }
        public string Url { get; set; }
    }
}
