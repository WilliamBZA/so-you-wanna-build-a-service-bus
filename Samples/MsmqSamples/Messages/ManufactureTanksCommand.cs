using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class ManufactureTanksCommand
    {
        public DateTime TimeSent { get; set; } = DateTime.Now;
        public string TankType { get; set; }
        public int NumberToManufacture { get; set; }
        public DateTime ManufactureByWhen { get; set; }
    }
}
