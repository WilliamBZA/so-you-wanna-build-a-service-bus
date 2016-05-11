using Messages.Contracts;
using Messages.ControlMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using ReflectionMagic;

namespace SimplePubSubBus
{
    public class Bus : IHandle<Subscribe>, IDisposable
    {
        private Dictionary<Type, List<string>> _typeMappings;
        private Dictionary<Type, List<Type>> _incomingHandlers;
        private List<dynamic> _handlerInstances;
        private MessageQueue _incomingQueue;

        public Bus(string incomingAddress)
        {
            _typeMappings = new Dictionary<Type, List<string>>();
            _handlerInstances = new List<dynamic>() { this.AsDynamic() };

            LoadHandlers();

            CheckQueueExists(incomingAddress);

            MapTypeToQueue(typeof(Subscribe), incomingAddress);

            _incomingQueue = new MessageQueue(incomingAddress);

            _incomingQueue.PeekCompleted += Queue_PeekCompleted;
            _incomingQueue.BeginPeek();

        }

        private static void CheckQueueExists(string queueName)
        {
            if (!MessageQueue.Exists(queueName))
            {
                using (MessageQueue.Create(queueName, true)) { }
            }
        }

        private void LoadHandlers()
        {
            var handleType = typeof(IHandle<>);

            _incomingHandlers = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 where assembly != typeof(DateTime).Assembly
                                 from type in assembly.GetTypes()
                                 where IsHandler(type)
                                 select new
                                 {
                                     HandlerType = type,
                                     Handles = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handleType).Select(i => i.GenericTypeArguments.First())
                                 }).ToDictionary(h => h.HandlerType, h => h.Handles.ToList());
        }

        private bool IsHandler(Type type)
        {
            var handleType = typeof(IHandle<>);

            if (handleType == type)
            {
                return false;
            }

            if (handleType.IsAssignableFrom(type))
            {
                return true;
            }

            return (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handleType));
        }

        public void Handle(Subscribe message)
        {
            MapTypeToQueue(message.MessageType, message.Address);
        }

        private void MapTypeToQueue(Type type, string queueAddress)
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

        private void Queue_PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            MessageQueue queue = (MessageQueue)sender;
            queue.EndPeek(e.AsyncResult);

            using (MessageQueueTransaction tx = new MessageQueueTransaction())
            {
                tx.Begin();

                var message = queue.Receive(tx);
                message.Formatter = new XmlMessageFormatter(_incomingHandlers.SelectMany(h => h.Value).Distinct().ToArray());

                var messageType = message.Body.GetType();
                var handlers = GetHandlersInstanceFor(messageType);

                foreach (var handler in handlers)
                {
                    handler.Handle(message.Body);
                }

                tx.Commit();
            }

            queue.BeginPeek();
        }

        private IEnumerable<dynamic> GetHandlersInstanceFor(Type messageType)
        {
            var handlers = _incomingHandlers.Where(h => h.Value.Any(type => type == messageType)).Select(h => h.Key);

            var missingHandlers = handlers.Where(handlerType => !_handlerInstances.Any(instance => instance.RealObject.GetType() == handlerType)).ToList();

            foreach (var missing in missingHandlers)
            {
                _handlerInstances.Add(Activator.CreateInstance(missing).AsDynamic());
            }

            return _handlerInstances.Where(instance => handlers.Any(handlerType => instance.RealObject.GetType() == handlerType));
        }

        private void SendMessageToQueue<T>(string destination, T message, MessageQueueTransaction tx)
        {
            using (var queue = new MessageQueue(destination))
            {
                queue.Send(message, tx);
            }
        }

        public void Dispose()
        {
            if (_incomingQueue != null)
            {
                try
                {
                    _incomingQueue.Dispose();
                }
                catch (Exception) { }

                _incomingQueue = null;
            }
        }
    }
}
