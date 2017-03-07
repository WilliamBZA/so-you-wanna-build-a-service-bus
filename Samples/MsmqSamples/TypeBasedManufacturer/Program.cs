using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TypeBasedBreakfast
{
    class Program
    {
        static Action<HaveBreakfastCommand> processMessage = message =>
        {
            Console.WriteLine("Must eat a {0} breakfast by {1}", message.BreakfastType, message.FinishByWhen);
        };

        static void Main(string[] args)
        {
            var breakfastAddress = @".\Private$\breakfast";

            using (var queue = new MessageQueue(breakfastAddress))
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
                message.Formatter = new XmlMessageFormatter(new Type[] { typeof(HaveBreakfastCommand) });

                var recievedMessage = (HaveBreakfastCommand)message.Body;
                processMessage(recievedMessage);

                tx.Commit();
            }

            queue.BeginPeek();
        }
    }
}