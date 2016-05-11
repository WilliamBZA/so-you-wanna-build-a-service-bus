using Messages;
using Messages.Contracts;
using SimplePubSubBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePubSubManufacturer
{
    class Program
    {
        static void Main(string[] args)
        {
            var publisherAddress = @".\Private$\simplePubSubSendIncoming";
            var incomingAddress = @".\Private$\simplePubSubSendManufacturer";

            var bus = new Bus(incomingAddress);
            bus.SubscribeToMessagesFrom<ManufactureTanksCommand>(publisherAddress);

            Console.ReadLine();
        }
    }

    public class Manufacturer : IHandle<ManufactureTanksCommand>
    {
        public void Handle(ManufactureTanksCommand message)
        {
            Console.WriteLine("Must manufature {0} {1} tanks by {2}", message.NumberToManufacture, message.TankType, message.ManufactureByWhen);
        }
    }
}