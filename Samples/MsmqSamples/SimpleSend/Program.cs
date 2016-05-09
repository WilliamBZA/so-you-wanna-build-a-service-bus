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
            var queueName = @".\Private$\msmqsimplesendsample";

            CheckQueueExists(queueName);

            using (var queue = new MessageQueue(queueName))
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