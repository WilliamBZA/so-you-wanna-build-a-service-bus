using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TypeBasedCentralCommand
{
    class Program
    {
        static Action<InvadeCountryCommand> processMessage = message =>
        {
            Console.WriteLine("Invade {0} on {1}", message.CountryToInvade, message.InvasionDate);
        };

        static void Main(string[] args)
        {
            var centralCommandAddress = @".\Private$\centralCommand";

            using (var queue = new MessageQueue(centralCommandAddress))
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
                message.Formatter = new XmlMessageFormatter(new Type[] { typeof(InvadeCountryCommand) });

                var recievedMessage = (InvadeCountryCommand)message.Body;
                processMessage(recievedMessage);

                tx.Commit();
            }

            queue.BeginPeek();
        }
    }
}