using KJDevSec.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Extensibility
{
    public interface IModule
    {
        string Name { get; }
        string Version { get; }
        int Priority { get; }
        void Up(IDependencyResolver resolver);
    }
}
