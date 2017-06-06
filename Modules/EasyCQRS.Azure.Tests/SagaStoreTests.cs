using EasyCQRS.Azure.EventSourcing;
using EasyCQRS.EventSourcing;
using EasyCQRS.Messaging;
using EasyCQRS.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyCQRS.Azure.Tests
{
    public class SagaStoreTests
    {
        [Fact]
        public void SagaStore_IsAssignableFromISagaStore()
        {
            var options = SetupContext();

            using(var context = new InfrastructureContext(options))
            {
                var sut = new SagaStore(Mock.Of<ICommandBus>(), Mock.Of<ISagaSerializer>(), context);

                Assert.IsAssignableFrom<ISagaStore>(sut);
            }
        }
        
        [Fact]
        public async Task SagaStore_FindAsyncDeserializesAndReturnsAppropiateSaga()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var stubSagaSerializer = new Mock<ISagaSerializer>();

                stubSagaSerializer.Setup(s => s.Deserialize<FakeSaga>(It.IsAny<byte[]>()))
                                  .Returns<byte[]>((v) =>
                                  {
                                      var payload = Encoding.UTF8.GetString(v);
                                      return JsonConvert.DeserializeObject<FakeSaga>(
                                          payload,
                                          new JsonSerializerSettings
                                          {
                                              TypeNameHandling = TypeNameHandling.All
                                          });
                                  });

                var sut = new SagaStore(Mock.Of<ICommandBus>(), stubSagaSerializer.Object, context);

                var saga = await sut.FindAsync<FakeSaga>(ToGuid(1));

                stubSagaSerializer.Verify(s => s.Deserialize<FakeSaga>(It.IsAny<byte[]>()));

                Assert.Equal(ToGuid(1), saga.Id);
            }
        }

        [Fact]
        public async Task SagaStore_FindAsyncWhenSagaIsCompletedReturnsNull()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var stubSagaSerializer = new Mock<ISagaSerializer>();

                stubSagaSerializer.Setup(s => s.Deserialize<FakeSaga>(It.IsAny<byte[]>()))
                              .Returns<byte[]>((v) =>
                              {
                                  var payload = Encoding.UTF8.GetString(v);
                                  return JsonConvert.DeserializeObject<FakeSaga>(
                                      payload, 
                                      new JsonSerializerSettings {
                                          TypeNameHandling = TypeNameHandling.All                                          
                                      });
                              });

                var sut = new SagaStore(Mock.Of<ICommandBus>(), stubSagaSerializer.Object, context);

                var saga = await sut.FindAsync<FakeSaga>(ToGuid(3));

                Assert.Null(saga);
            }
        }

        [Fact]
        public async Task SagaStore_FindAsyncWhenSagaHasPendingCommandPutThemToBus()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockBus = new Mock<ICommandBus>();

                var stubSagaSerializer = new Mock<ISagaSerializer>();
                stubSagaSerializer.Setup(s => s.Deserialize<FakeSaga>(It.IsAny<byte[]>()))
                                  .Returns<byte[]>((v) =>
                                  {
                                      var payload = Encoding.UTF8.GetString(v);
                                      return JsonConvert.DeserializeObject<FakeSaga>(
                                          payload,
                                          new JsonSerializerSettings
                                          {
                                              TypeNameHandling = TypeNameHandling.All
                                          });
                                  });

                var sut = new SagaStore(mockBus.Object, stubSagaSerializer.Object, context);

                await sut.FindAsync<FakeSaga>(ToGuid(2));

                mockBus.Verify(s => s.SendCommandAsync<Command>(It.IsAny<FakeCommand>()));
            }
        }       

        [Fact]
        public async Task SagaStore_SaveAsyncPersistChangesInContext()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var saga = new FakeSaga { Id = Guid.Empty };
                var sut = new SagaStore(Mock.Of<ICommandBus>(), Mock.Of<ISagaSerializer>(), context);

                await sut.SaveAsync(saga);

            }

            using (var context = new InfrastructureContext(options))
            {
                var sagaEntity = context.Sagas.FindAsync(Guid.Empty, typeof(FakeSaga).FullName);

                Assert.NotNull(sagaEntity);
            }
        }

        [Fact]
        public async Task SagaStore_SaveAsyncInvokesSerializeOnISagaSerializer()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var stubSagaSerializer = new Mock<ISagaSerializer>();

                stubSagaSerializer.Setup(s => s.Serialize(It.IsAny<FakeSaga>()))
                                  .Returns<FakeSaga>((s) =>
                                  {
                                      var payload = JsonConvert.SerializeObject(
                                          s,
                                          new JsonSerializerSettings
                                          {
                                              TypeNameHandling = TypeNameHandling.All
                                          });

                                      return Encoding.UTF8.GetBytes(payload);
                                  });

                var saga = new FakeSaga { Id = Guid.Empty };
                var sut = new SagaStore(Mock.Of<ICommandBus>(), stubSagaSerializer.Object, context);

                await sut.SaveAsync(saga);

                stubSagaSerializer.Verify(v => v.Serialize(saga));
            }
        }

        [Fact]
        public async Task SagaStore_SaveAsyncWhenPendingCommandsPutThemToBus()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockBus = new Mock<ICommandBus>();       

                var saga = new FakeSaga { Id = Guid.Empty, PendingCommands = { new FakeCommand() }};
                var sut = new SagaStore(mockBus.Object, Mock.Of<ISagaSerializer>(), context);

                await sut.SaveAsync(saga);

                mockBus.Verify(s => s.SendCommandAsync<Command>(It.IsAny<FakeCommand>()));
            }
        }

        [Fact]
        public async Task SagaStore_SaveAsyncWhenPendingCommandsPutThemToBusIfExceptionThrownSagaPersisted()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockBus = new Mock<ICommandBus>();
                mockBus.SetupSequence(s => s.SendCommandAsync<Command>(It.IsAny<FakeCommand>()))
                       .Returns(Task.CompletedTask)
                       .Throws(new DbUpdateException("Cannot save entity in db. Connection closed", new Exception()));

                var saga = new FakeSaga { Id = Guid.Empty, PendingCommands = { new FakeCommand(), new FakeCommand() } };
                var sut = new SagaStore(mockBus.Object, Mock.Of<ISagaSerializer>(), context);

                await Assert.ThrowsAsync<DbUpdateException>(async() => await sut.SaveAsync(saga));

                Assert.Equal(1, saga.PendingCommands.Count);
            }

            using(var context = new InfrastructureContext(options))
            {
                var sagaEntity = context.Sagas.FindAsync(Guid.Empty, typeof(SagaStore).FullName);

                Assert.NotNull(sagaEntity);
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

        private DbContextOptions<InfrastructureContext> PopulateContext()
        {
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                context.Sagas.Add(GenerateSagaEntity(ToGuid(1), false, false));
                context.Sagas.Add(GenerateSagaEntity(ToGuid(2), false, true));
                context.Sagas.Add(GenerateSagaEntity(ToGuid(3), true, false));
                context.Sagas.Add(GenerateSagaEntity(ToGuid(4), true, true));

                context.SaveChanges();
            }

            return options;
        }

        private SagaEntity GenerateSagaEntity(Guid sagaId, bool completed = false, bool pendingCommands = false)
        {
            var correlationId = Guid.NewGuid();
            var saga = new FakeSaga {
                Id = sagaId,
                Completed = completed,
                CorrelationId = correlationId,
                PendingCommands =
                {
                    new FakeCommand
                    {
                        CorrelationId = correlationId,
                        ExecutedBy = null,
                        PreviousCommandId = null
                    }
                }
            };

            if(!pendingCommands)
            {
                saga.PendingCommands?.Clear();
            }

            var json = JsonConvert.SerializeObject(saga, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            return new SagaEntity
            {
                CorrelationId = correlationId,
                Id = sagaId,
                Completed = completed,
                Type = typeof(FakeSaga).AssemblyQualifiedName,
                FullName = typeof(FakeSaga).FullName,
                Payload = Encoding.UTF8.GetBytes(json)
            };
        }

        private static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
    }
}
