using System;

namespace GuiStack.Models
{
    public class S3ObjectModel
    {
        public string BucketName { get; set; }
        public S3Object Object { get; set; }
    }
}
