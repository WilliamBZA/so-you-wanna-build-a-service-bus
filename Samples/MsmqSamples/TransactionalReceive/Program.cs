using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TransactionalReceive
{
    class Program
    {
        static Action<SimpleMessage> processMessage = message =>
        {
            Console.WriteLine("{0}\n\t sent at {1}", message.Title, message.TimeSent);
        };

        static void Main(string[] args)
        {
            var queueName = @".\Private$\msmqtransactionsample";

            using (var queue = new MessageQueue(queueName))
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
                message.Formatter = new XmlMessageFormatter(new Type[] { typeof(SimpleMessage) });

                var recievedMessage = (SimpleMessage)message.Body;
                processMessage(recievedMessage);

                tx.Commit();
            }

            queue.BeginPeek();
        }
    }
}
