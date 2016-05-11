using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTypeBasedRoutingSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var manufacturingAddress = @".\Private$\factoryCommand";
            var centralCommandAddress = @".\Private$\centralCommand";

            CheckQueueExists(manufacturingAddress);
            CheckQueueExists(centralCommandAddress);

            var sender = new Sender();

            // Mapping happens in sender. I.e. the sender MUST know where messages go.
            sender.MapTypeToQueue(typeof(InvadeCountryCommand), centralCommandAddress);
            sender.MapTypeToQueue(typeof(ManufactureTanksCommand), manufacturingAddress);

            sender.Send(new InvadeCountryCommand { CountryToInvade = "Poland", InvasionDate = DateTime.Now.AddMonths(1) });
            sender.Send(new ManufactureTanksCommand { TankType = "Hornsby tractor", NumberToManufacture = 5, ManufactureByWhen = DateTime.Now.AddMonths(1).AddDays(-7) });
        }

        private static void CheckQueueExists(string queueName)
        {
            if (!MessageQueue.Exists(queueName))
            {
                using (MessageQueue.Create(queueName, true)) { }
            }
        }
    }
}