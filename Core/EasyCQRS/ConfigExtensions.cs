using EasyCQRS.Diagnostics;
using EasyCQRS.EventSourcing;
using EasyCQRS.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public static class ConfigExtensions
    {
        public static IServiceCollection UseConsoleLogging(this IServiceCollection services)
        {
            return services.AddSingleton<ILogger, ConsoleLogger>();
        }

        public static IServiceCollection UseIntegrationEventMemoryBus(this IServiceCollection services)
        {
            return services.AddSingleton<IIntegrationEventBus, MemoryBus>();
        }

        public static IServiceCollection UseEventMemoryBus(this IServiceCollection services)
        {
            return services.AddSingleton<IEventBus, MemoryBus>();
        }

        public static IServiceCollection UseCommandMemoryBus(this IServiceCollection services)
        {
            return services.AddSingleton<ICommandBus, MemoryBus>();
        }

        public static IServiceCollection UseEasyCQRS(this IServiceCollection services)
        {
            return  services.AddSingleton<ICommandBus, MemoryBus>()
                            .AddSingleton<IEventBus, MemoryBus>()
                            .AddTransient<IRepository, Repository>()
                            .AddTransient<IAggregateSerializer, JsonAggregateSerializer>()
                            .AddTransient<IMessageSerializer, JsonMessageSerializer>()
                            .AddTransient<ISagaSerializer, JsonSagaSerializer>()
                            .AddTransient<ISnapshotStore, NullSnapshotStore>()
                            .AddTransient<ILogger, NullConsoleLogger>();
        }
    }
}
