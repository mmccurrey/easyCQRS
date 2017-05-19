using EasyCQRS.DI;
using EasyCQRS.EventSourcing;
using EasyCQRS.Extensibility;
using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{
    public class Azure : IModule
    {
        public string Name { get { return "Azure"; } }

        public int Priority { get { return 1; } }

        public string Version { get { return "0.1.0.0"; } }

        public void Up(IDependencyResolver resolver)
        {
            resolver.Register(() => new InfrastructureContext());
            resolver.Register<IBus, Messaging.ServiceBus>();
            resolver.Register<IEventStore, EventSourcing.EventStore>();
        }
    }
}
