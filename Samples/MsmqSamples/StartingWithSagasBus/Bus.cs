using Messages.Contracts;
using Messages.ControlMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using ReflectionMagic;
using System.Text;
using System.Threading.Tasks;

namespace StartingWithSagasBus
{
    public class Bus : IHandle<Subscribe>, IDisposable
    {
        private Dictionary<Type, List<string>> _typeMappings;
        private Dictionary<Type, List<Type>> _incomingHandlers;
        private Dictionary<Guid, SagaData> _sagaData;
        private List<dynamic> _handlerInstances;
        private MessageQueue _incomingQueue;

        public Bus(string incomingAddress)
        {
            _typeMappings = new Dictionary<Type, List<string>>();
            _sagaData = new Dictionary<Guid, SagaData>();
            _handlerInstances = new List<dynamic>() { this.AsDynamic() };

            LoadHandlers();

            CheckQueueExists(incomingAddress);

            MapTypeToQueue(typeof(Subscribe), incomingAddress);

            _incomingQueue = new MessageQueue(incomingAddress);

            _incomingQueue.PeekCompleted += Queue_PeekCompleted;
            _incomingQueue.BeginPeek();
        }

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
                                 where IsImplementationOf(handleType, type)
                                 select new
                                 {
                                     HandlerType = type,
                                     Handles = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handleType).Select(i => i.GenericTypeArguments.First())
                                 }).ToDictionary(h => h.HandlerType, h => h.Handles.ToList());
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
                    // if handler is saga and message.Body contains SagaId, populate data
                    if (IsImplementationOf(typeof(Saga<>), handler))
                    {
                        var bodyType = message.Body.GetType();
                        var sagaIdProperty = bodyType.GetProperties().FirstOrDefault(pi => pi.Name == "SagaId");
                        if (sagaIdProperty != null)
                        {
                            var sagaId = (Guid)sagaIdProperty.GetValue(message.Body);
                            if (sagaId != Guid.Empty)
                            {
                                if (!_sagaData.ContainsKey(sagaId))
                                {
                                    var sagaDataType = GetSagaDataTypeFor((Type)handler.RealObject.GetType());

                                    var instance = Activator.CreateInstance(sagaDataType);
                                    instance.AsDynamic().SagaId = sagaId;

                                    _sagaData.Add(sagaId, (SagaData)instance);
                                }

                                handler.Data = _sagaData[sagaId];
                            }
                        }
                    }

                    handler.Handle(message.Body);
                }

                tx.Commit();
            }

            queue.BeginPeek();
        }

        private Type GetSagaDataTypeFor(Type sagaType)
        {
            var sagaDataType = typeof(SagaData);

            return sagaType.BaseType.GenericTypeArguments.FirstOrDefault(type => sagaDataType.IsAssignableFrom(type));
        }

        private bool IsImplementationOf(Type expectedType, Type currentType)
        {
            if (expectedType == currentType)
            {
                return false;
            }

            if (expectedType.IsAssignableFrom(currentType))
            {
                return true;
            }

            return (currentType?.BaseType?.IsGenericType ?? false && currentType?.BaseType.GetGenericTypeDefinition() == expectedType) || (currentType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == expectedType));
        }

        private bool IsImplementationOf<T>(dynamic handler)
        {
            return IsImplementationOf(typeof(T), handler);
        }

        private bool IsImplementationOf(Type expectedType, dynamic handler)
        {
            return IsImplementationOf(expectedType, (Type)handler.RealObject.GetType());
        }

        private IEnumerable<dynamic> GetHandlersInstanceFor(Type messageType)
        {
            var handlers = _incomingHandlers.Where(h => h.Value.Any(type => type == messageType)).Select(h => h.Key);

            var missingHandlers = handlers.Where(handlerType => !_handlerInstances.Any(instance => instance.RealObject.GetType() == handlerType)).ToList();

            foreach (var missing in missingHandlers)
            {
                var constructor = missing.GetConstructor(new Type[] { });
                var ctorParameters = new object[] { };
                if (constructor == null)
                {
                    constructor = missing.GetConstructor(new Type[] { typeof(Bus) });
                    ctorParameters = new object[] { this };
                }

                if (constructor == null)
                {
                    throw new InvalidOperationException("No valid constructor found");
                }

                _handlerInstances.Add(constructor.Invoke(ctorParameters).AsDynamic());
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

            if (_handlerInstances != null)
            {
                var disposableHandlers = _handlerInstances.Where(h => h is IDisposable).Cast<IDisposable>();
                foreach (var disposable in disposableHandlers)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}