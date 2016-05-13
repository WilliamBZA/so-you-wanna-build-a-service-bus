using Messages;
using Messages.Contracts;
using StartingWithSagasBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaManufacturing
{
    class Program
    {
        static void Main(string[] args)
        {
            var manufacturingIncomingAddress = @".\Private$\simpleSagaManufacturing";
            var invasionSagaAddress = @".\Private$\simpleSagaInvasionSaga";

            var bus = new Bus(manufacturingIncomingAddress);
            bus.SubscribeToMessagesFrom<ManufactureTanksCommand>(invasionSagaAddress);

            Console.WriteLine("Subscriptions complete");
            Console.ReadLine();
        }
    }

    public class Manufacturer : IHandle<ManufactureTanksCommand>
    {
        private Bus _bus;

        public Manufacturer(Bus bus)
        {
            _bus = bus;
        }

        public void Handle(ManufactureTanksCommand message)
        {
            Console.WriteLine("Must manufature {0} {1} tanks by {2}", message.NumberToManufacture, message.TankType, message.ManufactureByWhen);

            Task.Delay(3000).Wait();

            _bus.Send(new TanksManufacturedEvent { SagaId = message.SagaId });
        }
    }
}