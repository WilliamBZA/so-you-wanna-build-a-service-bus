using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Contracts
{
    public abstract class Saga<T>
    {
        public void SetTimeout(DateTime timeoutAt)
        {
            throw new NotImplementedException();
        }

        public virtual void TimeoutTriggered(DateTime triggerTime)
        {
            throw new NotImplementedException();
        }

        public void MarkAsAborted()
        {
            throw new NotImplementedException();
        }

        public void MarkAsCompleted()
        {
            throw new NotImplementedException();
        }

        public T Data { get; set; }
    }
}