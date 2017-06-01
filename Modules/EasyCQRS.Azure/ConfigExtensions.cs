using EasyCQRS.Azure;
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
        public static Config UseAzure(this Config config)
        {
            config.Up(new AzureModule());

            return config;
        }
    }
}
