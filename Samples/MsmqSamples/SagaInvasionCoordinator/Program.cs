using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestSaga
{
    class Program
    {
        static void Main(string[] args)
        {
            var incomingAddress = @".\Private$\simpleSagaQuestSaga";
            var linkAddress = @".\Private$\simpleSagaLink";
            var badGuyAddress = @".\Private$\simpleSagaBadGuy";

            var bus = new Bus(incomingAddress);
            bus.SubscribeToMessagesFrom<PutPantsOnCommand>(linkAddress);
            bus.SubscribeToMessagesFrom<HaveBreakfastCommand>(linkAddress);

            bus.SubscribeToMessagesFrom<SwordPreparedEvent>(linkAddress);
            bus.SubscribeToMessagesFrom<PrincessKidnappedEvent>(badGuyAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class InvasionSaga : Saga<QuestSagaData>,
        IHandle<PutPantsOnCommand>,
        IHandle<HaveBreakfastCommand>,
        IHandle<SwordPreparedEvent>,
        IHandle<PrincessKidnappedEvent>
    {
        private Bus _bus;

        public InvasionSaga(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(PutPantsOnCommand message)
        {
            Console.WriteLine("Link has put his pants on. We should prepare his quest");

            _bus.Send(new KidnapPrincessCommand { SagaId = message.SagaId });
            _bus.Send(new PrepareSwordCommand { SagaId = message.SagaId, NumberOfFolds = 800 });

            // Check if everything is ready in time
            SetTimeout(DateTime.Now.AddSeconds(20));
        }

        public void Handle(HaveBreakfastCommand message)
        {
            if (!Data.IsSwordReady || !Data.IsPrincessKidnapped)
            {
                Console.WriteLine("Quest cancelled, telling everyone about it");
                _bus.Send(new CancelQuestCommand { });
            }

            MarkAsCompleted();
        }

        public void Handle(SwordPreparedEvent message)
        {
            Data.IsSwordReady = true;

            if (Data.IsPrincessKidnapped)
            {
                _bus.Send(new SaveThePrincessCommand { });
                MarkAsCompleted();
            }
        }

        public void Handle(PrincessKidnappedEvent message)
        {
            Data.IsPrincessKidnapped = true;

            if (Data.IsSwordReady)
            {
                _bus.Send(new SaveThePrincessCommand { });
                MarkAsCompleted();
            }
        }

        public override void TimeoutTriggered(DateTime triggerTime)
        {
            if (!Data.IsSwordReady || !Data.IsPrincessKidnapped)
            {
                Console.WriteLine("Quest cancelled, telling everyone about it");
                _bus.Send(new CancelQuestCommand { });
            }

            MarkAsCompleted();
        }
    }
}