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
        public Task PublishEventsAsync(params Event[] events)
        {
            if (@events != null)
            {
                foreach (var @event in events)
                {
                    SendMessage(@event);
                }
            }

            return Task.FromResult(0);
        }

        public Task SendCommandAsync<T>(T command) where T : Command
        {
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
