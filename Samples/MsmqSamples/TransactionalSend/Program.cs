using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TransactionalSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = @".\Private$\msmqtransactionsample";

            CheckQueueExists(queueName);

            using (var queue = new MessageQueue(queueName))
            {
                using (var tx = new MessageQueueTransaction())
                {
                    tx.Begin();

                    queue.Send(new SimpleMessage("Test message"), tx);

                    tx.Commit();
                }
            }
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
