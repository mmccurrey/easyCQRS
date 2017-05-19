using KJDevSec.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Messaging
{
    class MemoryBus : IBus
    {
        private readonly ILogger logger;

        public MemoryBus(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException("logger");
        }

        public Task PublishEventsAsync(params Event[] events)
        {
            if (@events != null)
            {
                foreach (var @event in events)
                {
                    logger.Info("[MemoryBus->PublishEventsAsync] Sending event of type: {0}", @event.GetType().Name);
                    SendMessage(@event);
                }
            }

            return Task.FromResult(0);
        }

        public Task SendCommandAsync<T>(T command) where T : Command
        {
            logger.Info("[MemoryBus->SendCommandAsync] Sending command of type: {0}", typeof(T).Name);
            return SendMessage(command);
        }

        private Task SendMessage<T>(T message) where T : IMessage
        {
            var handlers = Config.Container.ResolveAll(typeof(IHandler<>).MakeGenericType(message.GetType())).ToList();

            if (typeof(Event).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
            {
                return Task.WhenAll(handlers.Select(h => (Task)h.AsDynamic().HandleAsync(message)));
            }
            else
            {
                return (Task)handlers.First().AsDynamic().HandleAsync(message);
            }
        }
    }
}
