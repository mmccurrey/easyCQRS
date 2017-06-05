using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.Messaging
{
    public class IntegrationEventBus: CompositeEventBus
    {
        public IntegrationEventBus(IEventBus memoryBus, IEventBus serviceBus)
            : base(memoryBus, serviceBus)
        {
        }
    }
}
