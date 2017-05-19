using EasyCQRS.DI;
using EasyCQRS.Extensibility;
using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public static class Config
    {
        private static List<IModule> Modules = new List<IModule>();

        public static IDependencyResolver Container;
        
        static Config()
        {
            Container = new AutofactDependencyResolver(new Autofac.ContainerBuilder());

            RegisterDefaults();
        }        

        public static void Up(IModule module)
        {
            module.Up(Container);
            Modules.Add(module);
        }

        private static void RegisterDefaults()
        {
            Container.Register<IBus, MemoryBus>();
            Container.Register<IRepository, Repository>();
            Container.Register<IMessageSerializer, XmlMessageSerializer>();
            Container.Register<EventSourcing.ISnapshotStore, EventSourcing.NullSnapshotStore>();
            Container.Register<Diagnostics.ILogger, Diagnostics.ConsoleLogger>();
        }
    }
}
