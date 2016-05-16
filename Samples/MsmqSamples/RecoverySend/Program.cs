using Messages;
using RecoveryBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoverySend
{
    class Program
    {
        static void Main(string[] args)
        {
            var sendQueueAddress = @".\Private$\msmqrecoverySend";

            var bus = new Bus(sendQueueAddress);

            Console.WriteLine("Press enter to send message");
            Console.ReadLine();

            bus.Send(new SimpleMessage { });
            Console.WriteLine("Sent!");
            Console.ReadLine();
        }
    }
}