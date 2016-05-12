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

        }
    }

    public class InvasionSaga : Saga<InvasionSagaData>,
        IHandle<InvadeCountryCommand>,
        IHandle<CommenceInvasionCommand>,
        IHandle<TroopsPreparedEvent>,
        IHandle<TanksManufacturedEvent>
    {
        private Bus _bus;

        public void Handle(InvadeCountryCommand message)
        {
            _bus.Send(new ManufactureTanksCommand { });
            _bus.Send(new PrepareTroopsCommand { });

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
            _bus.Send(new DeployArmyCommand { });

            MarkAsCompleted();
        }

        public override void TimeoutTriggered(DateTime triggerTime)
        {
            if (Data.TroopsReady && Data.TanksReady)
            {
                _bus.Send(new CancelInvasionCommand { });
            }

            MarkAsCompleted();
        }
    }
}