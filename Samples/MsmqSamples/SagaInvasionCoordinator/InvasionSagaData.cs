using Messages.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaInvasionCoordinator
{
    public class InvasionSagaData : SagaData
    {
        public bool TroopsReady { get; set; }
        public bool TanksReady { get; set; }
    }
}