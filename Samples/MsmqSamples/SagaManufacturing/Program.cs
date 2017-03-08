using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaOldMan
{
    class Program
    {
        static void Main(string[] args)
        {
            var incomingAddress = @".\Private$\simpleSagaOldMan";
            var questSagaAddress = @".\Private$\simpleSagaQuestSaga";

            var bus = new Bus(incomingAddress);
            bus.SubscribeToMessagesFrom<PrepareSwordCommand>(questSagaAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class OldMan : IHandle<PrepareSwordCommand>
    {
        private Bus _bus;

        public OldMan(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(PrepareSwordCommand message)
        {
            Console.WriteLine("Must prepare the sword for Link's quest");

            Task.Delay(3000).Wait();

            Console.WriteLine("Sword is ready!");

            _bus.Publish(new SwordPreparedEvent { SagaId = message.SagaId });
        }
    }
}