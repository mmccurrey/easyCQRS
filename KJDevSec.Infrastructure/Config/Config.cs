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

        public static IDependencyResolver DIContainer;

        
        public static void Bootstrap()
        {
            //System.Runtime.Loader.AssemblyLoadContext

            var resolver = new AutofactDependencyResolver(new Autofac.ContainerBuilder());

            DIContainer = resolver;

            foreach(var module in Modules.OrderBy(m => m.Priority))
            {
                Console.WriteLine("Initializing module: {0}, version: {1}", module.Name, module.Version);
                module.Up(DIContainer);
            }

            /*
            PreloadAssemblies();

            GetModules().ForEach(m => m.Up(resolver));

            DiscoverHandlers().ForEach(h => {
                resolver.Register(h.Item1, h.Item2, Guid.NewGuid().ToString());
            });
            */
        }

        public static void RegisterModule(IModule module)
        {
            Modules.Add(module);
        }

        /*
        private static List<Type> GetTypes()
        {
            return (from a in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                    where a.FullName.StartsWith("KJDevSec")
                    from t in a.GetLoadableTypes()
                    select t).ToList();
        }

        private static void PreloadAssemblies()
        {
            Trace.Write("Getting Assemblies: ");
            List<Assembly> allAssemblies = new List<Assembly>();
            Assembly mainAssembly = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName(new Uri(mainAssembly.GetName().CodeBase).LocalPath);

            var leadboxAssemblies = (from a in mainAssembly.GetReferencedAssemblies().AsParallel()
                                     where a.FullName.StartsWith("Leadbox")
                                     select a).ToList();

            Trace.WriteLine("OK");

            Trace.Write("Preloading Assemblies: ");
            foreach (string dll in Directory.GetFiles(path, "Machete*.dll"))
            {
                var fileName = Path.GetFileNameWithoutExtension(dll);
                if (!leadboxAssemblies.Any(a => a.Name == fileName) && Assembly.GetExecutingAssembly().GetName().Name != fileName)
                {
                    allAssemblies.Add(Assembly.LoadFile(dll));
                }
            }

            Trace.WriteLine("OK");
        }

        private static List<Tuple<Type, Type>> DiscoverHandlers()
        {
            var handlerList = new List<Tuple<Type, Type>>();

            //Trace.Write("Discovering Handlers: ");
            var types = GetTypes();

            return (from type in types
                    let interfaces = type.GetInterfaces()
                    let handlerInterfaces = interfaces.Where(i =>
                                               i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<>))
                                            .ToArray()
                    where handlerInterfaces != null && handlerInterfaces.Count() > 0
                    select handlerInterfaces.Select(ht => new Tuple<Type, Type>(ht, type)))
                            .SelectMany(e => e)
                            .ToList();
        }

        private static List<IModule> GetModules()
        {
            var modules = new List<IModule>();


            Trace.Write("Discovering Modules: ");
            var types = GetTypes();

            var moduleTypes = (from t in types
                               let attribute = t.GetCustomAttribute<ModuleMetadataAttribute>()
                               where typeof(IModule).IsAssignableFrom(t) && t.IsClass
                               orderby attribute?.Order
                               select t)
                            .ToList();

            Trace.WriteLine("OK");
            foreach (var moduleType in moduleTypes)
            {
                Trace.WriteLine(String.Format("Bootstrapping {0} module", moduleType.Name.Replace("Module", "")));
                modules.Add((IModule)Activator.CreateInstance(moduleType));
            }

            return modules;
        }

    */
    }
}
