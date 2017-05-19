using EasyCQRS.DI;
using EasyCQRS.EventSourcing;
using EasyCQRS.Extensibility;
using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Autofac;

namespace EasyCQRS.AutoFac
{
    public class AutoFact : IModule
    {
        public string Name => "EasyCQRS.AutoFac";

        public int Priority => Int32.MinValue;

        public string Version => GetType().GetTypeInfo().Assembly.GetCustomAttribute<AssemblyVersionAttribute>().Version;

        public void Up(IDependencyResolver resolver)
        {
            Config.Container = new AutoFacDependencyResolver(new ContainerBuilder());
        }
    }
}
