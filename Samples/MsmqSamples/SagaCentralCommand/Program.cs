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

            bus.Publish(new PantsHaveBeenPutOnEvent { PantsColour = "Green", SagaId = sagaId });

            Console.WriteLine("Pants on. Press enter to eat breakfast");
            Console.ReadLine();

            bus.Publish(new BreakfastFinishedEvent { BreakfastType= "Full English", SagaId = sagaId });

            Console.ReadLine();
        }
    }

    public class Link : IHandle<SaveThePrincessCommand>
    {
        public void Handle(SaveThePrincessCommand message)
        {
            Console.WriteLine("Go forth and save the kingdom!");
        }
    }
}