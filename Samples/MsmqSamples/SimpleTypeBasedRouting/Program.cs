using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTypeBasedRoutingSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var breakfastAddress = @".\Private$\breakfast";
            var linkAddress = @".\Private$\link";

            CheckQueueExists(breakfastAddress);
            CheckQueueExists(linkAddress);

            var sender = new Sender();

            // Mapping happens in sender. I.e. the sender MUST know where messages go.
            sender.MapTypeToQueue(typeof(PutPantsOnCommand), linkAddress);
            sender.MapTypeToQueue(typeof(HaveBreakfastCommand), breakfastAddress);

            sender.Send(new PutPantsOnCommand { PantsColour = "Green", ManufactureDate = DateTime.Now.AddMonths(-1) });
            sender.Send(new HaveBreakfastCommand { BreakfastType = "Full English", NumberOfCoffees = 5, FinishByWhen = DateTime.Now.AddHours(1) });
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