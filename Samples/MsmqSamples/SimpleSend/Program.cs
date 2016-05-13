using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueAddress = @".\Private$\msmqsimplesendsample";

            CheckQueueExists(queueAddress);

            using (var queue = new MessageQueue(queueAddress))
            {
                queue.Send(new SimpleMessage("Test message"));
            }
        }

        private static void CheckQueueExists(string queueName)
        {
            if (!MessageQueue.Exists(queueName))
            {
                using (MessageQueue.Create(queueName, false)) { }
            }
        }
    }
}