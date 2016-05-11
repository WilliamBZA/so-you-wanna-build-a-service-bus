using Messages;
using Messages.Contracts;
using SimplePubSubBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePubSubCentralCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            var incomingAddress = @".\Private$\simplePubSubSendCentralCommand";

            var bus = new Bus(incomingAddress);
        }
    }

    public class CentralCommand : IHandle<InvadeCountryCommand>
    {
        public void Handle(InvadeCountryCommand message)
        {
            throw new NotImplementedException();
        }
    }
}