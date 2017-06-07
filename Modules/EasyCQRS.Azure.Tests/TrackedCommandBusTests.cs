using EasyCQRS.Azure.Messaging;
using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using EasyCQRS.Tests;
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
    public class TrackedCommandBusTests
    {
        [Fact]
        public void TrackedCommandBus_IsAssignableFromICommandBus()
        {
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {

                var sut = new TrackedCommandBus(
                                            Mock.Of<IServiceProvider>(),
                                            Mock.Of<IMessageSerializer>(),
                                            Mock.Of<ILogger>(),
                                            context);

                Assert.IsAssignableFrom<ICommandBus>(sut);
            }
        }

        [Fact]
        public async Task TrackedCommandBus_SendCommandAsyncPersistsChangesToContext()
        {
            var command = new FakeCommand();
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                var sut = new TrackedCommandBus(
                                         Mock.Of<IServiceProvider>(),
                                         Mock.Of<IMessageSerializer>(),
                                         Mock.Of<ILogger>(),
                                         context);


                await sut.SendCommandAsync(command);
            }

            using (var context = new InfrastructureContext(options))
            {
                var commandEntity = await context.Commands.FindAsync(command.CommandId);

                Assert.NotNull(commandEntity);
                Assert.Equal(command.CommandId, commandEntity.Id);
                Assert.Equal(command.CorrelationId, commandEntity.CorrelationId);
            }
        }

        [Fact]
        public async Task TrackedCommandBus_SendCommandAsyncInvokesSerializeOnIMessageSerializer()
        {
            var command = new FakeCommand();
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IMessageSerializer>();
                var sut = new TrackedCommandBus(
                                         Mock.Of<IServiceProvider>(),
                                         mockSerializer.Object,
                                         Mock.Of<ILogger>(),
                                         context);


                await sut.SendCommandAsync(command);

                mockSerializer.Verify(s => s.Serialize(command));
            }
        }

        [Fact]
        public async Task TrackedCommandBus_SendCommandAsyncRetrievesHandlerFromServiceProvider()
        {
            var command = new FakeCommand();
            var options = SetupContext();
            var fakeHandler = Mock.Of<IHandler<FakeCommand>>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider.Setup(m => m.GetService(typeof(IHandler<FakeCommand>)))
                               .Returns(fakeHandler);

            using (var context = new InfrastructureContext(options))
            {
                var sut = new TrackedCommandBus(
                                         Mock.Of<IServiceProvider>(),
                                         Mock.Of<IMessageSerializer>(),
                                         Mock.Of<ILogger>(),
                                         context);


                await sut.SendCommandAsync(command);
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
