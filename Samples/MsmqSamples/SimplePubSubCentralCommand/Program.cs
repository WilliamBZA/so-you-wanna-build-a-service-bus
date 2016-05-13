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
            var publisherAddress = @".\Private$\simplePubSubSendIncoming";
            var incomingAddress = @".\Private$\simplePubSubSendCentralCommand";

            var bus = new Bus(incomingAddress);
            bus.SubscribeToMessagesFrom<InvadeCountryCommand>(publisherAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class CentralCommand : IHandle<InvadeCountryCommand>
    {
        public void Handle(InvadeCountryCommand message)
        {
            Console.WriteLine("Invade {0} on {1}", message.CountryToInvade, message.InvasionDate);
        }
    }
}