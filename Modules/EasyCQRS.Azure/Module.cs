using EasyCQRS.DI;
using EasyCQRS.EventSourcing;
using EasyCQRS.Extensibility;
using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{
    public class Azure : IModule
    {
        public string Name => "EasyCQRS.Azure";

        public int Priority => Int32.MinValue;

        public string Version => GetType().GetTypeInfo().Assembly.GetCustomAttribute<AssemblyVersionAttribute>().Version;

        public void Up(Config config)
        {
            config.Container.Register(() => new InfrastructureContext());
            config.Container.Register<IBus, Messaging.ServiceBus>();
            config.Container.Register<IEventStore, EventSourcing.EventStore>();
        }
    }
}
