using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.ControlMessages
{
    public class Subscribe
    {
        public Type MessageType { get; set; }
        public string Address { get; set; }
    }
}