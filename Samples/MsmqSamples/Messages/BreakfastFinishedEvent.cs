using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class BreakfastFinishedEvent
    {
        public DateTime TimeSent { get; set; } = DateTime.Now;
        public string BreakfastType { get; set; }
        public int NumberOfCoffees { get; set; }
        public DateTime FinishByWhen { get; set; }
        public Guid SagaId { get; set; }
    }
}
