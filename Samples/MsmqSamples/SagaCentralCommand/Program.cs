using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaCentralCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            var centralCommandIncomingAddress = @".\Private$\simpleSagaCentralCommand";
            var invasionSagaAddress = @".\Private$\simpleSagaInvasionSaga";
            var churchillAddress = @".\Private$\simpleSagaChurchill";

            var bus = new Bus(centralCommandIncomingAddress);
            bus.SubscribeToMessagesFrom<PrepareTroopsCommand>(invasionSagaAddress);
            bus.SubscribeToMessagesFrom<DeployArmyCommand>(invasionSagaAddress);
            bus.SubscribeToMessagesFrom<CommenceInvasionCommand>(churchillAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class CentralCommand : IHandle<InvadeCountryCommand>,
        IHandle<PrepareTroopsCommand>,
        IHandle<CommenceInvasionCommand>
    {
        private Bus _bus;

        public CentralCommand(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(CommenceInvasionCommand message)
        {
            Console.WriteLine("Dispatching troops...");
        }

        public void Handle(InvadeCountryCommand message)
        {
            Console.WriteLine("Invade {0} on {1}", message.CountryToInvade, message.InvasionDate);
        }

        public void Handle(PrepareTroopsCommand message)
        {
            Console.WriteLine("Prepare {0} on {1}", message.NumberOfTroops, message.InvasionDate);

            Task.Delay(3000).Wait();

            _bus.Send(new TroopsPreparedEvent { SagaId = message.SagaId });
        }
    }
}