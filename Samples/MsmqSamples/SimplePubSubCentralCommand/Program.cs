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
            bus.SubscribeToMessagesFrom<PantsHaveBeenPutOnEvent>(publisherAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class PantsHandler : IHandle<PantsHaveBeenPutOnEvent>
    {
        public void Handle(PantsHaveBeenPutOnEvent message)
        {
            Console.WriteLine("{0} pants have been put on", message.PantsColour);
        }
    }
}