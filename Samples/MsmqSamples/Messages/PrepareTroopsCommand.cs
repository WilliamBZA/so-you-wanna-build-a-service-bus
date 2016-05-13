using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class PrepareTroopsCommand
    {
        public DateTime InvasionDate { get; set; }
        public int NumberOfTroops { get; set; }
        public Guid SagaId { get; set; }
    }
}