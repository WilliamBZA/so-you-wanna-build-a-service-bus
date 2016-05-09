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
        static void Main(string[] args)
        {
            var queueName = @".\Private$\msmqsimplesendsample";

            using (var queue = new MessageQueue(queueName))
            {
                Message message = null;
                while ((message = queue.Receive()) != null)
                {
                    message.Formatter = new XmlMessageFormatter(new Type[] { typeof(SimpleMessage) });

                    var recievedMessage = (SimpleMessage)message.Body;
                    Console.WriteLine("{0}\n\t recieved at {1}", recievedMessage.Title, recievedMessage.TimeSent);
                };
            }
        }
    }
}