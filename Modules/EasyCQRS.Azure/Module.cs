using EasyCQRS.DI;
using EasyCQRS.EventSourcing;
using EasyCQRS.Extensibility;
using EasyCQRS.Messaging;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{
    public class Azure : IModule
    {
        public string Name => "EasyCQRS.Azure";

        public int Priority => Int32.MinValue;

        public string Version => GetType().GetTypeInfo().Assembly.GetCustomAttribute<AssemblyVersionAttribute>().Version;

        public void Up(Config config)
        {
            var configurationManager = config.Container.Resolve<IConfigurationManager>();            

            config.Container.Register(() => new InfrastructureContext());
            config.Container.Register<IBus, Messaging.ServiceBus>();
            config.Container.Register<IEventSubscriber, Messaging.ServiceBusEventSubscriber>();
            config.Container.Register<IEventStore, EventSourcing.EventStore>();
            config.Container.Register<ISagaStore, EventSourcing.SagaStore>();
            config.Container.Register(() =>
            {
                var tenantId = configurationManager.GetSetting("Microsoft.Azure.TenantId");
                var subscriptionId = configurationManager.GetSetting("Microsoft.Azure.SubscriptionId");
                var clientId = configurationManager.GetSetting("Microsoft.Azure.ClientId");
                var clientSecret = configurationManager.GetSetting("Microsoft.Azure.ClientSecret");

                var context = new AuthenticationContext("https://login.windows.net/" + tenantId);
                var result = context.AcquireTokenAsync(
                                        "https://management.core.windows.net/",
                                        new ClientCredential(clientId, clientSecret)
                                      )
                                      .GetAwaiter()
                                      .GetResult();

                var creds = new TokenCredentials(result.AccessToken);
                return new ServiceBusManagementClient(creds, new RetryDelegatingHandler())
                {
                    SubscriptionId = subscriptionId                   
                };
            });
        }
    }
}
