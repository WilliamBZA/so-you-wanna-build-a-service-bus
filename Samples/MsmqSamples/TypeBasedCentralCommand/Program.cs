using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TypeBasedPantsCommand
{
    class Program
    {
        static Action<PutPantsOnCommand> processMessage = message =>
        {
            Console.WriteLine("Putting {0} pants on", message.PantsColour);
        };

        static void Main(string[] args)
        {
            var linkAddress = @".\Private$\link";

            using (var queue = new MessageQueue(linkAddress))
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
                message.Formatter = new XmlMessageFormatter(new Type[] { typeof(PutPantsOnCommand) });

                var receivedMessage = (PutPantsOnCommand)message.Body;
                processMessage(receivedMessage);

                tx.Commit();
            }

            queue.BeginPeek();
        }
    }
}
