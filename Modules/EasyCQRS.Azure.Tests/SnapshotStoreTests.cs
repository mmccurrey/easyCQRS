using EasyCQRS.Azure.EventSourcing;
using EasyCQRS.EventSourcing;
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
    public class SnapshotStoreTests
    {
        [Fact]
        public void SnapshotStore_IsAssignableFromISnapshotStore()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                Assert.IsAssignableFrom<ISnapshotStore>(new SnapshotStore(Mock.Of<IAggregateSerializer>(), context));
            }
        }

        [Fact]
        public async Task SnapshotStore_FindAsyncReturnsInvokesDeserializeOnIAggregateSerializer()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IAggregateSerializer>();
                mockSerializer.Setup(s => s.Deserialize<FakeAggregate>(It.IsAny<byte[]>()))
                              .Returns<byte[]>(bytes =>
                              {
                                  var payload = Encoding.UTF8.GetString(bytes);
                                  return JsonConvert.DeserializeObject<FakeAggregate>(
                                      payload,
                                      new JsonSerializerSettings
                                      {
                                          ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                          TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                                          TypeNameHandling = TypeNameHandling.All,
                                          ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
                                      });
                              });

                var sut = new SnapshotStore(mockSerializer.Object, context);

                var aggregate = await sut.GetByIdAsync<FakeAggregate>(ToGuid(1));

                mockSerializer.Setup(s => s.Deserialize<FakeAggregate>(It.IsAny<byte[]>()));
            }
        }

        [Fact]
        public async Task SnapshotStore_FindAsyncReturnsAppropiateAggregate()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IAggregateSerializer>();
                mockSerializer.Setup(s => s.Deserialize<FakeAggregate>(It.IsAny<byte[]>()))
                              .Returns<byte[]>(bytes =>
                              {
                                  var payload = Encoding.UTF8.GetString(bytes);
                                  return JsonConvert.DeserializeObject<FakeAggregate>(
                                      payload,
                                      new JsonSerializerSettings
                                      {
                                          ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                          TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                                          TypeNameHandling = TypeNameHandling.All,
                                          ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
                                      });
                              });

                var sut = new SnapshotStore(mockSerializer.Object, context);

                var aggregate = await sut.GetByIdAsync<FakeAggregate>(ToGuid(1));

                Assert.Equal(ToGuid(1), aggregate.Id);
                Assert.Equal(20, aggregate.Version);
            }
        }

        [Fact]
        public async Task SnapshotStore_FindAsyncWithMaxVersionReturnsInvokesDeserializeOnIAggregateSerializer()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IAggregateSerializer>();
                mockSerializer.Setup(s => s.Deserialize<FakeAggregate>(It.IsAny<byte[]>()))
                              .Returns<byte[]>(bytes =>
                              {
                                  var payload = Encoding.UTF8.GetString(bytes);
                                  return JsonConvert.DeserializeObject<FakeAggregate>(
                                      payload,
                                      new JsonSerializerSettings
                                      {
                                          ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                          TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                                          TypeNameHandling = TypeNameHandling.All,
                                          ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
                                      });
                              });

                var sut = new SnapshotStore(mockSerializer.Object, context);

                var aggregate = await sut.GetByIdAsync<FakeAggregate>(ToGuid(1), 10);

                mockSerializer.Setup(s => s.Deserialize<FakeAggregate>(It.IsAny<byte[]>()));
            }
        }

        [Fact]
        public async Task SnapshotStore_FindAsyncWithMaxVersionReturnsAppropiateAggregate()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IAggregateSerializer>();
                mockSerializer.Setup(s => s.Deserialize<FakeAggregate>(It.IsAny<byte[]>()))
                              .Returns<byte[]>(bytes =>
                              {
                                  var payload = Encoding.UTF8.GetString(bytes);
                                  return JsonConvert.DeserializeObject<FakeAggregate>(
                                      payload,
                                      new JsonSerializerSettings
                                      {
                                          ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                          TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                                          TypeNameHandling = TypeNameHandling.All,
                                          ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
                                      });
                              });

                var sut = new SnapshotStore(mockSerializer.Object, context);

                var aggregate = await sut.GetByIdAsync<FakeAggregate>(ToGuid(1), 10);

                Assert.Equal(ToGuid(1), aggregate.Id);
                Assert.Equal(10, aggregate.Version);
            }
        }

        [Fact]
        public async Task SnapshotStore_SaveAsyncInvokesSerializeOnIAggregateSerializer()
        {
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IAggregateSerializer>();
                mockSerializer.Setup(s => s.Serialize(It.IsAny<FakeAggregate>()))
                              .Returns<FakeAggregate>(aggr =>
                              {
                                  var payload = JsonConvert.SerializeObject(
                                         aggr,
                                         new JsonSerializerSettings
                                         {
                                             ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                             TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                                             TypeNameHandling = TypeNameHandling.All,
                                             ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
                                         });


                                  return Encoding.UTF8.GetBytes(payload);                                  
                              });

                var sut = new SnapshotStore(mockSerializer.Object, context);
                var aggregate = new FakeAggregate(Guid.Empty);

                await sut.SaveAsync(aggregate);

                mockSerializer.Setup(s => s.Serialize(It.IsAny<FakeAggregate>()));
            }
        }

        [Fact]
        public async Task SnapshotStore_SaveAsyncPersistChangesToContext()
        {
            var options = SetupContext();

            using (var context = new InfrastructureContext(options))
            {
                var mockSerializer = new Mock<IAggregateSerializer>();
                mockSerializer.Setup(s => s.Serialize(It.IsAny<FakeAggregate>()))
                              .Returns<FakeAggregate>(aggr =>
                              {
                                  var payload = JsonConvert.SerializeObject(
                                         aggr,
                                         new JsonSerializerSettings
                                         {
                                             ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                             TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                                             TypeNameHandling = TypeNameHandling.All,
                                             ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
                                         });


                                  return Encoding.UTF8.GetBytes(payload);
                              });

                var sut = new SnapshotStore(mockSerializer.Object, context);
                var aggregate = new FakeAggregate(Guid.Empty, 50L);

                await sut.SaveAsync(aggregate);
            }

            using (var context = new InfrastructureContext(options))
            {
                var entity = await context.Snapshots.FindAsync(typeof(FakeAggregate).FullName, Guid.Empty, 50L);

                Assert.NotNull(entity);
                Assert.Equal(Guid.Empty, entity.AggregateId);
                Assert.Equal(50, entity.Version);
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
                context.Snapshots.Add(GenerateSnapshotEntity(ToGuid(1), 10));
                context.Snapshots.Add(GenerateSnapshotEntity(ToGuid(1), 20));
                context.Snapshots.Add(GenerateSnapshotEntity(ToGuid(2), 10));
                context.Snapshots.Add(GenerateSnapshotEntity(ToGuid(2), 20));
                context.Snapshots.Add(GenerateSnapshotEntity(ToGuid(2), 30));

                context.SaveChanges();
            }

            return options;
        }

        private SnapshotEntity GenerateSnapshotEntity(Guid aggregateId, long version)
        {
            return new SnapshotEntity
            {
                SourceType = typeof(FakeAggregate).FullName,
                AggregateId = aggregateId,
                Version = version,
                Payload = Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(
                                new FakeAggregate(aggregateId, version),
                                new JsonSerializerSettings
                                {
                                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                                    TypeNameHandling = TypeNameHandling.All,
                                    ContractResolver = new Serialization.NonPublicPropertiesContractResolver()
                                }))
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
