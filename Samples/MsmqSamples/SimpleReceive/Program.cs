using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReceive
{
    public class Program
    {
        static Action<SimpleMessage> processMessage = message =>
        {
            Console.WriteLine("{0}\n\t sent at {1}", message.Title, message.TimeSent);
        };

        static void Main(string[] args)
        {
            var queueAddress = @".\Private$\msmqsimplesendsample";

            using (var queue = new MessageQueue(queueAddress))
            {
                queue.ReceiveCompleted += Queue_ReceiveCompleted;
                queue.BeginReceive();

                Console.ReadLine();
            }
        }

        private static void Queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            MessageQueue queue = (MessageQueue)sender;
            var message = queue.EndReceive(e.AsyncResult);
            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(SimpleMessage) });

            var recievedMessage = (SimpleMessage)message.Body;
            processMessage(recievedMessage);

            queue.BeginReceive();
        }
    }
}