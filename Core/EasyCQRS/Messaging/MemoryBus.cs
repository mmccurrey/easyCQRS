using EasyCQRS.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public class MemoryBus : ICommandBus, IEventBus, IIntegrationEventBus
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public MemoryBus(IServiceProvider serviceProvider, ILogger logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException("serviceProvider");
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

        public Task PublishEventsAsync(params IntegrationEvent[] events)
        {
            if (@events != null)
            {
                foreach (var @event in events)
                {
                    logger.Info("[MemoryBus->PublishEventsAsync] Sending integration event of type: {0}", @event.GetType().Name);
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
            var handlerType = typeof(IHandler<>).MakeGenericType(message.GetType());
            var handlers = serviceProvider.GetServices(handlerType);

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
