using Messages;
using Messages.Contracts;
using SimplePubSubBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePubSubPants
{
    class Program
    {
        static void Main(string[] args)
        {
            var publisherAddress = @".\Private$\simplePubSubSendIncoming";
            var incomingAddress = @".\Private$\simplePubSubSendCentralCommand";

            var bus = new Bus(incomingAddress);
            bus.SubscribeToMessagesFrom<PutPantsOnCommand>(publisherAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class CentralCommand : IHandle<PutPantsOnCommand>
    {
        public void Handle(PutPantsOnCommand message)
        {
            Console.WriteLine("Putting {0} pants on", message.PantsColour);
        }
    }
}