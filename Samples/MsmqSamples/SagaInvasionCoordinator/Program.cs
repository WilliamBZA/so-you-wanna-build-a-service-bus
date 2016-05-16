using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaInvasionCoordinator
{
    class Program
    {
        static void Main(string[] args)
        {
            var invasionSagaIncomingAddress = @".\Private$\simpleSagaInvasionSaga";
            var churchillAddress = @".\Private$\simpleSagaChurchill";
            var manufacturingAddress = @".\Private$\simpleSagaManufacturing";
            var centralCommandAddress = @".\Private$\simpleSagaCentralCommand";

            var bus = new Bus(invasionSagaIncomingAddress);
            bus.SubscribeToMessagesFrom<InvadeCountryCommand>(churchillAddress);
            bus.SubscribeToMessagesFrom<CommenceInvasionCommand>(churchillAddress);

            bus.SubscribeToMessagesFrom<TroopsPreparedEvent>(centralCommandAddress);
            bus.SubscribeToMessagesFrom<TanksManufacturedEvent>(manufacturingAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class InvasionSaga : Saga<InvasionSagaData>,
        IHandle<InvadeCountryCommand>,
        IHandle<CommenceInvasionCommand>,
        IHandle<TroopsPreparedEvent>,
        IHandle<TanksManufacturedEvent>
    {
        private Bus _bus;

        public InvasionSaga(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(InvadeCountryCommand message)
        {
            Console.WriteLine("Invasion of {0} scheduled for {1}", message.CountryToInvade, message.InvasionDate);

            _bus.Send(new ManufactureTanksCommand { SagaId = message.SagaId, ManufactureByWhen = message.InvasionDate.AddMonths(-1), NumberToManufacture = 50 });
            _bus.Send(new PrepareTroopsCommand { SagaId = message.SagaId, InvasionDate = message.InvasionDate, NumberOfTroops = 800 });

            //SetTimeout(DateTime.Now.AddSeconds(20));
            SetTimeout(message.InvasionDate.AddMonths(-1));
        }

        public void Handle(TroopsPreparedEvent message)
        {
            Data.TroopsReady = true;

            if (Data.TanksReady)
            {
                _bus.Send(new InvasionForceReadyEvent { });
            }
        }

        public void Handle(TanksManufacturedEvent message)
        {
            Data.TanksReady = true;

            if (Data.TroopsReady)
            {
                _bus.Send(new InvasionForceReadyEvent { });
            }
        }

        public void Handle(CommenceInvasionCommand message)
        {
            MarkAsCompleted();
        }

        public override void TimeoutTriggered(DateTime triggerTime)
        {
            if (!Data.TroopsReady || !Data.TanksReady)
            {
                Console.WriteLine("Invasion cancelled, telling everyone about it");
                _bus.Send(new CancelInvasionCommand { });
            }

            MarkAsCompleted();
        }
    }
}