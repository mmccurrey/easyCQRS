using KJDevSec.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KJDevSec.DI;
using System.Reflection;

namespace KJDevSec
{
    class Module : IModule
    {
        public string Name { get { return "Infrastructure"; } }

        public int Priority { get { return Int32.MinValue; } }

        public string Version { get { return this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString(); } }

        public void Up(IDependencyResolver resolver)
        {
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += AssemblyLoadContextUploading;
        }

        private void AssemblyLoadContextUploading(System.Runtime.Loader.AssemblyLoadContext obj)
        {
            
        }
    }
}
