﻿using EasyCQRS.Azure;
using EasyCQRS.Azure.EventSourcing;
using EasyCQRS.Azure.Messaging;
using EasyCQRS.Diagnostics;
using EasyCQRS.EventSourcing;
using EasyCQRS.Messaging;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EasyCQRS
{
    public static class ConfigExtensions
    {
        public static IServiceCollection UseAzureForEasyCQRS(this IServiceCollection services)
        {
            return  services.AddDbContext<InfrastructureContext>((provider, options) =>
                            {
                                var configurationManager = provider.GetService<IConfigurationManager>();
                                var connectionString = configurationManager.GetSetting("EasyCQRS.Infrastructure.ConnectionString");

                                options.UseSqlServer(connectionString,
                                                     sqlServerOptionsAction: sqlOptions =>
                                                     {
                                                         sqlOptions.MigrationsAssembly(typeof(InfrastructureContext).GetTypeInfo().Assembly.GetName().Name);
                                                         sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                                     });
                                
                                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
                            })
                            .AddTransient<ServiceBus>()
                            .AddTransient<IEventBus, IntegrationEventBus>(p =>
                            {
                                var logger = p.GetService<ILogger>();
                                var serviceBus = p.GetService<ServiceBus>();
                                var memoryBus = new MemoryBus(p, logger);
                                return new IntegrationEventBus(memoryBus, serviceBus);
                            })
                            .AddTransient<IEventSubscriber, ServiceBusEventSubscriber>()
                            .AddTransient<IEventStore, EventStore>()
                            .AddTransient<ISagaStore, SagaStore>()
                            .AddTransient<IServiceBusManagementClient>((p) =>
                            {
                                var configurationManager = p.GetService<IConfigurationManager>();
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
                            })
                            .AddTransient((p) =>
                            {
                                var configurationManager = p.GetService<IConfigurationManager>();
                                return ServiceBusHelper.GetCommandsQueueClient(configurationManager);
                            })
                            .AddTransient((p) =>
                            {
                                var configurationManager = p.GetService<IConfigurationManager>();
                                var mgmtClient = p.GetService<IServiceBusManagementClient>();

                                return ServiceBusHelper.GetEventsTopicClient(configurationManager, mgmtClient);
                            });
        }
    }
}
