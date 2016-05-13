using Messages.Contracts;
using Messages.ControlMessages;
using ReflectionMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace RecoveryBus
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
            CheckQueueExists(incomingAddress + ".error");

            MapTypeToQueue(typeof(Subscribe), incomingAddress);

            _incomingQueue = new MessageQueue(incomingAddress);

            _incomingQueue.PeekCompleted += Queue_PeekCompleted;
            _incomingQueue.BeginPeek();
        }

        public bool FlrIsEnabled { get; set; }
        public bool SlrIsEnabled { get; set; }
        public bool DlqIsEnabled { get; set; }

        public void SubscribeToMessagesFrom<T>(string fromAddress)
        {
            using (var tx = new MessageQueueTransaction())
            {
                using (var queue = new MessageQueue(fromAddress))
                {
                    tx.Begin();
                    queue.Send(new Subscribe { MessageType = typeof(T).AssemblyQualifiedName, Address = _incomingQueue.Path }, tx);
                    tx.Commit();
                }
            }
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
            MapTypeToQueue(Type.GetType(message.MessageType), message.Address);
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

        private void SendToErrorQueue(object message)
        {
            using (var tx = new MessageQueueTransaction())
            {
                tx.Begin();

                var destination = _incomingQueue.Path + ".error";
                using (var queue = new MessageQueue(destination))
                {
                    queue.Send(message, tx);
                }

                tx.Commit();
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
                    HandleMessage(message.Body, handler);
                }

                tx.Commit();
            }

            queue.BeginPeek();
        }

        private void HandleMessage(object body, dynamic handler, int retryCount = 0, int slrRetryCount = 0)
        {
            try
            {
                handler.Handle(body);
            }
            catch (Exception) when (FlrIsEnabled && retryCount++ < 2)
            {
                HandleMessage(body, handler, retryCount);
            }
            catch (Exception) when (SlrIsEnabled && slrRetryCount++ < 3)
            {
                var delayTime = slrRetryCount * 1000;
                Console.WriteLine("Delaying SLR for {0} ms", delayTime);
                Task.Delay(delayTime).Wait();
                HandleMessage(body, handler, retryCount, slrRetryCount);
            }
            catch (Exception) when (DlqIsEnabled)
            {
                Console.WriteLine("Ahhh! Couldn't resolve!");
                SendToErrorQueue(body);
            }
            catch (Exception)
            {
                throw;
            }
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
                    _incomingQueue.PeekCompleted -= Queue_PeekCompleted;
                    _incomingQueue.Dispose();
                }
                catch (Exception) { }

                _incomingQueue = null;
            }
        }
    }
}
