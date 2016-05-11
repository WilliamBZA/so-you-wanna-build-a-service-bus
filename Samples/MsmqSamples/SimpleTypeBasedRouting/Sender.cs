using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTypeBasedRoutingSend
{
    public class Sender
    {
        private Dictionary<Type, List<string>> _typeMappings;

        public Sender()
        {
            _typeMappings = new Dictionary<Type, List<string>>();
        }

        public void MapTypeToQueue(Type type, string queueAddress)
        {
            if (!_typeMappings.ContainsKey(type))
            {
                _typeMappings.Add(type, new List<string>());
            }

            _typeMappings[type].Add(queueAddress);
        }

        public void Send<T>(T message)
        {
            List<string> destinations;
            if (_typeMappings.TryGetValue(typeof(T), out destinations))
            {
                using (var tx = new MessageQueueTransaction())
                {
                    tx.Begin();

                    foreach (var destination in destinations)
                    {
                        SendMessageToQueue(destination, message, tx);
                    }

                    tx.Commit();
                }
            }
        }

        private void SendMessageToQueue<T>(string destination, T message, MessageQueueTransaction tx)
        {
            using (var queue = new MessageQueue(destination))
            {
                queue.Send(message, tx);
            }
        }
    }
}