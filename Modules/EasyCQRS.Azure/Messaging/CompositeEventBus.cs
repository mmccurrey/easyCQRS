using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    public class CompositeEventBus : IEventBus
    {
        private readonly IEventBus[] buses;

        public CompositeEventBus(params IEventBus[] buses)
        {
            this.buses = buses ?? throw new ArgumentNullException("buses");
        }

        public async Task PublishEventsAsync(params Event[] events)
        {
            foreach(var bus in buses)
            {
                await bus.PublishEventsAsync(events);
            }            
        }
    }
}
