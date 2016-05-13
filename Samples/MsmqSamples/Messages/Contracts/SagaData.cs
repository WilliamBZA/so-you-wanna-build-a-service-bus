using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Contracts
{
    public abstract class SagaData
    {
        public Guid SagaId { get; set; }
    }
}