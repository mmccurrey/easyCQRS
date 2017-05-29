using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public static class ConfigExtensions
    {
        public static Config UseDefaultContainer(this Config config)
        {
            config.ReplaceIoCContainer(null);

            return config;
        }

        public static Config UseConsoleLogging(this Config config)
        {
            config.Container.Register<ILogger, ConsoleLogger>();

            return config;
        }

        public static Config UseMemoryBus(this Config config)
        {
            config.Container.Register<IBus, MemoryBus>();

            return config;
        }
    }
}
