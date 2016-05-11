using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class SimpleMessage
    {
        public SimpleMessage() { }

        public SimpleMessage(string title)
        {
            Title = title;
            TimeSent = DateTime.Now;
        }

        public string Title { get; set; }
        public DateTime TimeSent { get; set; } = DateTime.Now;
    }
}
