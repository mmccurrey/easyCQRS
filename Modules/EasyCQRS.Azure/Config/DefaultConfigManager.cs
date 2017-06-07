using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Azure.Config
{
    class DefaultConfigurationManager : IConfigurationManager
    {
        private readonly IConfigurationRoot configurationRoot;

        public DefaultConfigurationManager(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot ?? throw new ArgumentNullException("configurationRoot");
        }

        public string GetSetting(string name)
        {
            name = name?.Replace(".", ":");

            return configurationRoot[name];
        }
    }
}
