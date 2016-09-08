using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaBadGuy
{
    class Program
    {
        static void Main(string[] args)
        {
            var incomingAddress = @".\Private$\simpleSagaBadGuy";
            var questSagaAddress = @".\Private$\simpleSagaQuestSaga";

            var bus = new Bus(incomingAddress);
            bus.SubscribeToMessagesFrom<KidnapPrincessCommand>(questSagaAddress);
            bus.SubscribeToMessagesFrom<CancelQuestCommand>(questSagaAddress);
            bus.SubscribeToMessagesFrom<LinkIsComingEvent>(questSagaAddress);

            Console.WriteLine("Subscriptions complete");
            Console.WriteLine("Press enter to begin invasion");
            Console.ReadLine();


            Console.WriteLine("Invasion command sent!");
            Console.ReadLine();
        }
    }

    public class BadGuy : IHandle<CancelQuestCommand>,
        IHandle<KidnapPrincessCommand>,
        IHandle<LinkIsComingEvent>
    {
        private Bus _bus;

        public BadGuy(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(KidnapPrincessCommand message)
        {
            Console.WriteLine("Kidnapping princess... Please hold.");
            Task.Delay(3000).Wait();

            Console.WriteLine("Princess has been kidnapped. She's fiesty :)");

            _bus.Send(new PrincessKidnappedEvent { });
        }

        public void Handle(LinkIsComingEvent message)
        {
            Console.WriteLine("Expecting Link. Sending minions to defend.");
        }

        public void Handle(CancelQuestCommand message)
        {
            Console.WriteLine("Invasion has been cancelled :'(");
        }
    }
}