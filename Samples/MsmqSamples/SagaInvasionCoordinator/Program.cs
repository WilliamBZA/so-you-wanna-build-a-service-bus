using Messages;
using Messages.Contracts;
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

    public class InvasionSaga : IHandle<InvadeCountryCommand>,
        IHandle<ManufactureTanksCommand>
    {
        public void Handle(InvadeCountryCommand message)
        {
            SetTimeout();
            throw new NotImplementedException();
        }

        private void SetTimeout()
        {
            throw new NotImplementedException();
        }

        public void Handle(ManufactureTanksCommand message)
        {
            throw new NotImplementedException();
        }

        public void Handle(TanksManufacturedEvent message)
        {

        }

        public /*override*/ void TimeTriggered(DateTime triggerTime)
        {

            MarkAsAborted();
            MarkAsCompleted();
        }


        // saga methods
        private void MarkAsAborted()
        {
            throw new NotImplementedException();
        }

        private void MarkAsCompleted()
        {
            throw new NotImplementedException();
        }
    }
}