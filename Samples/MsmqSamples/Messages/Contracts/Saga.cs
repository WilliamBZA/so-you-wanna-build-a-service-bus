using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace Messages.Contracts
{
    public abstract class Saga<T> : IDisposable where T : SagaData
    {
        private List<Timer> _timeouts = new List<Timer>();

        public void SetTimeout(DateTime timeoutAt)
        {
            var days = (int)Math.Floor((timeoutAt - DateTime.Now).TotalDays);
            var triggerTime = (timeoutAt - DateTime.Now).TotalMilliseconds;
            if (days > 0)
            {
                triggerTime = TimeSpan.FromDays(1).TotalMilliseconds;
            }

            var timeout = new Timer(triggerTime);
            timeout.Elapsed += (sender, e) =>
            {
                if (--days <= 0)
                {
                    timeout.Stop();
                    TimeoutTriggered(timeoutAt);

                    _timeouts.Remove(timeout);
                    try
                    {
                        timeout.Dispose();
                    }
                    catch { }
                }
            };

            _timeouts.Add(timeout);
            timeout.Start();
        }

        public virtual void TimeoutTriggered(DateTime triggerTime)
        {
        }

        public void Dispose()
        {
            foreach (var timeout in _timeouts)
            {
                if (timeout != null)
                {
                    try
                    {
                        timeout.Dispose();
                    }
                    catch { }
                }
            }
        }

        public T Data { get; set; }
    }
}