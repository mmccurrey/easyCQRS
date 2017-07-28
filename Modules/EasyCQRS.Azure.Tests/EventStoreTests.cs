using EasyCQRS.Azure.EventSourcing;
using EasyCQRS.EventSourcing;
using EasyCQRS.Messaging;
using EasyCQRS.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyCQRS.Azure.Tests
{
    public class EventStoreTests
    {
        public static Guid Aggregate1 = ToGuid(1);
        public static Guid Aggregate2 = ToGuid(2);

        [Fact]
        public void EventStore_IsAssignableFromIEventStore()
        {
            var options = SetupContext();
            
            using (var context = new InfrastructureContext(options))
            {
                var sut = new EventStore(Mock.Of<IHttpContextAccessor>(), Mock.Of<IMessageSerializer>(), context);

                Assert.IsAssignableFrom<IEventStore>(sut);
            }
        }

        [Fact]
        public async Task EventStore_NoChangesPersistedUntilSaveChangesAsyncCalled()
        {
            var options = SetupContext();
            
            using (var context = new InfrastructureContext(options))
            {
                var sut = new EventStore(Mock.Of<IHttpContextAccessor>(), Mock.Of<IMessageSerializer>(), context);

                sut.Save(typeof(FakeAggregate), new FakeEvent(Guid.Empty, 1, "Fake Value"));

                Assert.False(await context.Events.AnyAsync());

                await sut.SaveChangesAsync();
            }

            using (var context = new InfrastructureContext(options))
            {
                Assert.True(await context.Events.AnyAsync());
            }
        }

        [Fact]
        public async Task EventStore_EventEntityHasPropertiesWithCorrectValues()
        {
            var options = SetupContext();
            var fakeEvent = new FakeEvent(Guid.Empty, 1, "Fake Value");

            using (var context = new InfrastructureContext(options))
            {
                var sut = new EventStore(Mock.Of<IHttpContextAccessor>(), Mock.Of<IMessageSerializer>(), context);

                sut.Save(typeof(FakeAggregate), fakeEvent);
                
                await sut.SaveChangesAsync();
            }

            using (var context = new InfrastructureContext(options))
            {
                var eventEntity = context.Events.FirstOrDefault();

                Assert.Equal(fakeEvent.AggregateId, eventEntity.AggregateId);
                Assert.Equal(fakeEvent.Version, eventEntity.Version);
                Assert.Equal(typeof(FakeAggregate).FullName, eventEntity.SourceType);
                Assert.Equal(typeof(FakeEvent).AssemblyQualifiedName, eventEntity.Type);
                Assert.Equal(typeof(FakeEvent).FullName, eventEntity.FullName);
            }
        }

        [Fact]
        public async Task EventStore_LoadAsyncReturnsAppropiateEvents()
        {
            var options = PopulateContext();
            
            using (var context = new InfrastructureContext(options))
            {
                var sut = new EventStore(Mock.Of<IHttpContextAccessor>(), Mock.Of<IMessageSerializer>(), context);

                var eventEntities = await sut.LoadAsync<FakeAggregate>(Aggregate1);

                Assert.Equal(4, eventEntities.Count());
            }            
        }

        [Fact]
        public async Task EventStore_LoadAsyncWithLastKnownVersionParameterReturnsAppropiateEvents()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var sut = new EventStore(Mock.Of<IHttpContextAccessor>(), Mock.Of<IMessageSerializer>(), context);

                var eventEntities = await sut.LoadAsync<FakeAggregate>(Aggregate1, 2);

                Assert.Equal(2, eventEntities.Count());
            }
        }

        [Fact]
        public async Task EventStore_LoadAsyncWithMaxVersionReturnsAppropiateEvents()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var sut = new EventStore(Mock.Of<IHttpContextAccessor>(), Mock.Of<IMessageSerializer>(), context);

                var eventEntities = await sut.LoadByMaxVersionAsync<FakeAggregate>(Aggregate1, 1);

                Assert.Equal(1, eventEntities.Count());
            }
        }

        [Fact]
        public async Task EventStore_LoadAsyncWithMaxVersionAndLastKnownVersionReturnsAppropiateEvents()
        {
            var options = PopulateContext();

            using (var context = new InfrastructureContext(options))
            {
                var sut = new EventStore(Mock.Of<IHttpContextAccessor>(), Mock.Of<IMessageSerializer>(), context);

                var eventEntities = await sut.LoadByMaxVersionAsync<FakeAggregate>(Aggregate1, 1, 3);

                Assert.Equal(2, eventEntities.Count());
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
                context.Events.Add(GenerateEventEntity(Aggregate1, 1));
                context.Events.Add(GenerateEventEntity(Aggregate1, 2));
                context.Events.Add(GenerateEventEntity(Aggregate1, 3));
                context.Events.Add(GenerateEventEntity(Aggregate1, 4));

                context.Events.Add(GenerateEventEntity(Aggregate2, 1));
                context.Events.Add(GenerateEventEntity(Aggregate2, 2));
                context.Events.Add(GenerateEventEntity(Aggregate2, 3));
                context.Events.Add(GenerateEventEntity(Aggregate2, 4));

                context.SaveChanges();
            }

            return options;
        }

        private EventEntity GenerateEventEntity(Guid aggregateId, long version)
        {
            var time = DateTimeOffset.UtcNow;
            var correlationId = Guid.NewGuid().ToString();
            var eventId = Guid.NewGuid();
            var json = $@"{{ 
                          ""EventId"": ""{eventId}"",
                          ""AggregateId"": ""{aggregateId}"",
                          ""CorrelationId"": ""{correlationId}"",
                          ""Version"": {version},
                          ""Timestamp"": ""{time}"",
                          ""ExecutedBy"": null,
                          ""Value"": ""{eventId}_Fake Value""
                        }}";

            return new EventEntity
            {
                AggregateId = aggregateId,
                Version = version,
                CorrelationId = correlationId,
                Date = time,
                Payload = Encoding.UTF8.GetBytes(json),
                SourceType = typeof(FakeAggregate).FullName,
                Type = typeof(FakeEvent).FullName
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
