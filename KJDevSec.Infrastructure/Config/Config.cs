using KJDevSec.DI;
using KJDevSec.Extensibility;
using KJDevSec.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec
{
    public static class Config
    {
        private static List<IModule> Modules = new List<IModule>();

        public static IDependencyResolver Container;

        public static IMessageSerializer MessageSerializer;

        public static ISettingManager SettingManager;

        static Config()
        {
            Container = new AutofactDependencyResolver(new Autofac.ContainerBuilder());
            MessageSerializer = new XmlMessageSerializer();
        }        

        public static void Up(IModule module)
        {
            module.Up(Container);
            Modules.Add(module);
        }

        public static string GetSetting(string name)
        {
            return SettingManager.GetSetting(name);
        }
        
    }
}
