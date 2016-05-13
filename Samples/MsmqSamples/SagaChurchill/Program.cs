using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaChurchill
{
    class Program
    {
        static void Main(string[] args)
        {
            var churchillIncomingAddress = @".\Private$\simpleSagaChurchill";
            var invasionSagaAddress = @".\Private$\simpleSagaInvasionSaga";

            var bus = new Bus(churchillIncomingAddress);
            bus.SubscribeToMessagesFrom<InvasionForceReadyEvent>(invasionSagaAddress);
            bus.SubscribeToMessagesFrom<CancelInvasionCommand>(invasionSagaAddress);

            Console.WriteLine("Subscriptions complete");
            Console.WriteLine("Press enter to begin invasion");
            Console.ReadLine();

            bus.Send(new InvadeCountryCommand { CountryToInvade = "France", InvasionDate = DateTime.Now.AddMonths(6), SagaId = Guid.NewGuid() });

            Console.WriteLine("Invasion command sent!");
            Console.ReadLine();
        }
    }

    public class Churchill : IHandle<CancelInvasionCommand>,
        IHandle<InvasionForceReadyEvent>
    {
        private Bus _bus;

        public Churchill(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(CancelInvasionCommand message)
        {
            Console.WriteLine("Invasion has been cancelled :'(");
        }

        public void Handle(InvasionForceReadyEvent message)
        {
            Console.WriteLine("Invasion force is ready :)");

            _bus.Send(new CommenceInvasionCommand { });
        }
    }
}