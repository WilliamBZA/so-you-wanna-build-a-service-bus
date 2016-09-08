using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaLink
{
    class Program
    {
        static void Main(string[] args)
        {
            var linkIncomingAddress = @".\Private$\simpleSagaLink";

            var sagaAddress = @".\Private$\simpleSagaQuestSaga";

            var bus = new Bus(linkIncomingAddress);
            bus.SubscribeToMessagesFrom<SaveThePrincessCommand>(sagaAddress);

            var sagaId = Guid.NewGuid();

            Console.WriteLine("Subscriptions complete");
            Console.WriteLine("Press enter to put pants on");
            Console.ReadLine();

            bus.Send(new PutPantsOnCommand { PantsColour = "Green", SagaId = sagaId });

            Console.WriteLine("Pants on. Press enter to each breakfast");
            Console.ReadLine();

            bus.Send(new HaveBreakfastCommand { BreakfastType= "Full English", SagaId = sagaId });

            Console.ReadLine();
        }
    }

    public class Link : IHandle<SaveThePrincessCommand>
    {
        private Bus _bus;

        public Link(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(SaveThePrincessCommand message)
        {
            Console.WriteLine("Go forth and save the kingdom!");
        }
    }
}