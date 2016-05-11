using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class InvadeCountryCommand
    {
        public DateTime TimeSent { get; set; } = DateTime.Now;
        public string CountryToInvade { get; set; }
        public DateTime InvasionDate { get; set; }
    }
}