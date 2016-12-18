using KJDevSec.DI;
using KJDevSec.EventSourcing;
using KJDevSec.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Azure
{
    public class Azure : IModule
    {
        public string Name { get { return "Azure"; } }

        public int Priority { get { return 1; } }

        public string Version { get { return "0.1.0.0"; } }

        public void Up(IDependencyResolver resolver)
        {
            resolver.Register(() => new EventSourcing.SQLEventSourcingContext());
            resolver.Register<IEventStore, EventSourcing.SQLEventStore>();
        }
    }
}
