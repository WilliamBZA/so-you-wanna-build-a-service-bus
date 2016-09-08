using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class PrepareSwordCommand
    {
        public DateTime QuestDate { get; set; }
        public int NumberOfFolds { get; set; }
        public Guid SagaId { get; set; }
    }
}