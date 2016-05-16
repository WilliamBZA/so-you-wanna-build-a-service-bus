using Messages;
using Messages.Contracts;
using Messages.ControlMessages;
using SimplePubSubBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePubSubSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var incomingAddress = @".\Private$\simplePubSubSendIncoming";

            var bus = new Bus(incomingAddress);
            
            Console.WriteLine("Press enter to send messages");
            Console.ReadLine();

            bus.Send(new InvadeCountryCommand { CountryToInvade = "Poland", InvasionDate = DateTime.Now.AddMonths(1) });
            bus.Send(new ManufactureTanksCommand { TankType = "Hornsby tractor", NumberToManufacture = 5, ManufactureByWhen = DateTime.Now.AddMonths(1).AddDays(-7) });

            Console.WriteLine("Messages sent!");

            Console.ReadLine();
        }
    }
}