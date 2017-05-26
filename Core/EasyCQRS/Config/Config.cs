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
    public class Config
    {        
        private IDependencyResolver resolver;
        private ICollection<IModule> Modules = new List<IModule>();

        public IDependencyResolver Container { get; private set; }       
        public Dictionary<Type, Type> Services { get; private set; }

        public Config()
        {
            RegisterDefaultServices();
            RegisterIoCContainer();
        }

        public void Up(IModule module)
        {
            module.Up(this);
            Modules.Add(module);
        }

        public void ReplaceIoCContainer(IDependencyResolver dependencyResolver)
        {
            if(dependencyResolver == null)
            {
                throw new ArgumentNullException("dependencyResolver");
            }

            if(Container != dependencyResolver)
            {
                RegisterIoCContainer(dependencyResolver);
            }
        }

        void RegisterDefaultServices()
        {
            if(this.Services == null)
            {
                this.Services = new Dictionary<Type, Type>();
            }

            RegisterService<IBus, MemoryBus>();
            RegisterService<IRepository, Repository>();
            RegisterService<IMessageSerializer, XmlMessageSerializer>();
            RegisterService<EventSourcing.ISnapshotStore, EventSourcing.NullSnapshotStore>();
            RegisterService<Diagnostics.ILogger, Diagnostics.ConsoleLogger>();
        }

        void RegisterService<TInterface, TImplementation>() where TImplementation : TInterface
        {
            this.Services.Add(typeof(TInterface), typeof(TImplementation));
        }

        void RegisterIoCContainer(IDependencyResolver dependencyResolver = null)
        {
            Container = dependencyResolver ?? new DefaultDependencyResolver();
            Container.Register(() => Container);

            foreach(var service in Services)
            {
                Container.Register(service.Key, service.Value);
            }
        }
    }
}
