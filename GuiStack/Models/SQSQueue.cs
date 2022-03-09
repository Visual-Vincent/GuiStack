using System;
using System.Linq;

namespace GuiStack.Models
{
    public class SQSQueue
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public SQSQueue()
        {
        }

        public SQSQueue(string url)
        {
            Uri uri = new Uri(url);

            Url = uri.ToString();
            Name = string.Join("", uri.Segments.Skip(2));
        }
    }
}
