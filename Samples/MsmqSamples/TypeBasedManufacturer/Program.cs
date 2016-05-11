using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TypeBasedManufacturer
{
    class Program
    {
        static Action<ManufactureTanksCommand> processMessage = message =>
        {
            Console.WriteLine("Must manufature {0} {1} tanks by {2}", message.NumberToManufacture, message.TankType, message.ManufactureByWhen);
        };

        static void Main(string[] args)
        {
            var manufacturingAddress = @".\Private$\factoryCommand";

            using (var queue = new MessageQueue(manufacturingAddress))
            {
                queue.PeekCompleted += Queue_PeekCompleted;
                queue.BeginPeek();

                Console.ReadLine();
            }
        }

        private static void Queue_PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            MessageQueue queue = (MessageQueue)sender;

            using (MessageQueueTransaction tx = new MessageQueueTransaction())
            {
                tx.Begin();

                var message = queue.Receive(tx);
                message.Formatter = new XmlMessageFormatter(new Type[] { typeof(ManufactureTanksCommand) });

                var recievedMessage = (ManufactureTanksCommand)message.Body;
                processMessage(recievedMessage);

                tx.Commit();
            }

            queue.BeginPeek();
        }
    }
}