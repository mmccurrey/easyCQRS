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
        public Task SendCommandAsync<T>(T command) where T : Command
        {
            return SendMessage(command);
        }

        public Task SendEventAsync<T>(T @event) where T : Event
        {
            return SendMessage(@event);
        }

        private Task SendMessage<T>(T message) where T : IMessage
        {
            var handlers = Config.DIContainer.ResolveAll(typeof(IHandler<>).MakeGenericType(message.GetType())).ToList();

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
