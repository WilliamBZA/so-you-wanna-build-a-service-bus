using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DeleteQueues
{
    class Program
    {
        static void Main(string[] args)
        {
            var queues = MessageQueue.GetPrivateQueuesByMachine(".");

            foreach (var queue in queues)
            {
                MessageQueue.Delete(queue.Path);
            }
        }
    }
}