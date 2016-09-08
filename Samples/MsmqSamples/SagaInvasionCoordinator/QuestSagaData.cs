using Messages.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestSaga
{
    public class QuestSagaData : SagaData
    {
        public bool IsSwordReady { get; set; }
        public bool IsPrincessKidnapped { get; set; }
    }
}