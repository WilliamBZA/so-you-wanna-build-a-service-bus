using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class PutPantsOnCommand
    {
        public DateTime TimeSent { get; set; } = DateTime.Now;
        public string PantsColour { get; set; }
        public DateTime ManufactureDate { get; set; }
        public Guid SagaId { get; set; }
    }
}