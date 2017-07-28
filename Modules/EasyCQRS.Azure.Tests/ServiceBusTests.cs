using EasyCQRS.Azure.Messaging;
using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using EasyCQRS.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyCQRS.Azure.Tests
{
    public class ServiceBusTests
    {
        [Fact]
        public void ServiceBus_IsAssignableFromIIntegrationEventBus()
        {
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {

                var sut = new ServiceBus(
                                            Mock.Of<IMessageSerializer>(),
                                            Mock.Of<IHttpContextAccessor>(),
                                            Mock.Of<IQueueClient>(),
                                            Mock.Of<ITopicClient>(),
                                            Mock.Of<ILogger>(),
                                            context);

                Assert.IsAssignableFrom<IIntegrationEventBus>(sut);
            }
        }        

        [Fact]
        public async Task ServiceBus_PublishIntegrationEventsAsyncPersistsChangesToContext()
        {
            var @event = new FakeIntegrationEvent("Fake Value");
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IMessageSerializer>();
                var sut = new ServiceBus(
                                         Mock.Of<IMessageSerializer>(),
                                         Mock.Of<IHttpContextAccessor>(),
                                         Mock.Of<IQueueClient>(),
                                         Mock.Of<ITopicClient>(),
                                         Mock.Of<ILogger>(),
                                         context);


                await sut.PublishEventsAsync(@event);
            }

            using (var context = new InfrastructureContext(options))
            {
                var eventEntity = await context.IntegrationEvents.FindAsync(@event.MessageId);

                Assert.NotNull(eventEntity);
                Assert.Equal(@event.MessageId, eventEntity.Id);
            }
        }

        [Fact]
        public async Task ServiceBus_PublishIntegrationEventsAsyncInvokesSerializeOnIMessageSerializer()
        {
            var @event = new FakeIntegrationEvent("Fake Value");
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IMessageSerializer>();
                var sut = new ServiceBus(mockSerializer.Object,
                                         Mock.Of<IHttpContextAccessor>(),
                                         Mock.Of<IQueueClient>(),
                                         Mock.Of<ITopicClient>(),
                                         Mock.Of<ILogger>(),
                                         context);


                await sut.PublishEventsAsync(@event);

                mockSerializer.Verify(s => s.Serialize<IntegrationEvent>(@event));
            }
        }

        [Fact]
        public async Task ServiceBus_PublishIntegrationEventsAsyncInvokesSendAsyncOnIQueueClient()
        {
            var @event = new FakeIntegrationEvent("Fake Value");
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockClient = new Mock<ITopicClient>();
                var sut = new ServiceBus(
                                         Mock.Of<IMessageSerializer>(),
                                         Mock.Of<IHttpContextAccessor>(),
                                         Mock.Of<IQueueClient>(),
                                         mockClient.Object,
                                         Mock.Of<ILogger>(),
                                         context);


                await sut.PublishEventsAsync(@event);

                mockClient.Verify(s => s.SendAsync(It.IsAny<Microsoft.Azure.ServiceBus.Message>()));
            }
        }

        private DbContextOptions<InfrastructureContext> SetupContext()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                                        .AddEntityFrameworkInMemoryDatabase()
                                        .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<InfrastructureContext>()
                                    .UseInMemoryDatabase()
                                    .UseInternalServiceProvider(serviceProvider);

            var options = builder.Options;

            // Create the schema in the database
            using (var context = new InfrastructureContext(options))
            {
                context.Database.EnsureCreated();
            }

            return options;
        }
    }
}
