using EasyCQRS.Diagnostics;
using EasyCQRS.EventSourcing;
using EasyCQRS.Messaging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyCQRS.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public async Task OnSaveChangesAsync_EventsPublishedToBus()
        {
            var fakeAggregate = new FakeAggregate(Guid.Empty);

            var mockBus = new Mock<IBus>();
            
            mockBus.Setup(t => t.PublishEventsAsync(It.IsAny<FakeEvent>())).Returns(Task.FromResult(0));

            var sut = new Repository(
                Mock.Of<IEventStore>(),
                Mock.Of<ISnapshotStore>(),
                mockBus.Object,
                Mock.Of<ILogger>()
                );

            fakeAggregate.Fire("Fake Value");

            sut.Save(fakeAggregate);

            await sut.SaveChangesAsync();

            mockBus.Verify(t => t.PublishEventsAsync(It.IsAny<FakeEvent>()));
        }

        [Fact]
        public async Task OnSaveChangesAsync_EventsSentToEventStore()
        {
            var mockEventStore = new Mock<IEventStore>();            
            var fakeAggregate = new FakeAggregate(Guid.Empty);
            
            mockEventStore.Setup(t => t.Save(typeof(FakeAggregate), It.IsAny<FakeEvent>()));
            mockEventStore.Setup(t => t.SaveChangesAsync()).Returns(Task.FromResult(0));
            
            var sut = new Repository(
                mockEventStore.Object,
                Mock.Of<ISnapshotStore>(),
                Mock.Of<IBus>(),
                Mock.Of<ILogger>()
                );

            fakeAggregate.Fire("Fake Value");

            sut.Save(fakeAggregate);

            await sut.SaveChangesAsync();

            mockEventStore.Verify(t => t.Save(typeof(FakeAggregate), (It.IsAny<FakeEvent>())));
            mockEventStore.Verify(t => t.SaveChangesAsync());
        }
        
        [Fact]
        public async Task OnFindAsync_BuildAggregateFromEventStore()
        {
            var mockEventStore = new Mock<IEventStore>();            

            mockEventStore.Setup(t => t.LoadAsync<FakeAggregate>(Guid.Empty)).ReturnsAsync(Enumerable.Empty<Event>());

            var sut = new Repository(
                mockEventStore.Object,
                Mock.Of<ISnapshotStore>(),
                Mock.Of<IBus>(),
                Mock.Of<ILogger>()                
                );
            
            var aggregate = await sut.GetByIdAsync<FakeAggregate>(Guid.Empty);

            mockEventStore.Verify(t => t.LoadAsync<FakeAggregate>(Guid.Empty));
        }

        [Fact]
        public async Task OnFindAsync_BuildAggregateFromSnapshotStore()
        {
            var fakeAggregate = new FakeAggregate(Guid.Empty);            
            var mockSnapshotStore = new Mock<ISnapshotStore>();
           
            mockSnapshotStore.Setup(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty)).ReturnsAsync(fakeAggregate);

            var sut = new Repository(
                Mock.Of<IEventStore>(),
                mockSnapshotStore.Object,
                Mock.Of<IBus>(),
                Mock.Of<ILogger>()
                );

            var aggregate = await sut.GetByIdAsync<FakeAggregate>(Guid.Empty);

            mockSnapshotStore.Verify(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty));

            Assert.Same(fakeAggregate, aggregate);
        }

        [Fact]
        public async Task OnFindAsync_BuildAggregateFromEventStoreAndSnapshotStore()
        {
            var fakeAggregate = new FakeAggregate(Guid.Empty);
            var fakeEvent = new FakeEvent(Guid.NewGuid(), Guid.Empty, 2, null, "Fake Value");

            var mockEventStore = new Mock<IEventStore>();
            var mockSnapshotStore = new Mock<ISnapshotStore>();

            mockSnapshotStore.Setup(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty)).ReturnsAsync(fakeAggregate);
            mockEventStore.Setup(t => t.LoadAsync<FakeAggregate>(Guid.Empty, 0)).ReturnsAsync(Enumerable.Repeat(fakeEvent, 1));

            var sut = new Repository(
                mockEventStore.Object,
                mockSnapshotStore.Object,
                Mock.Of<IBus>(),
                Mock.Of<ILogger>()
                );

            var aggregate = await sut.GetByIdAsync<FakeAggregate>(Guid.Empty);

            mockSnapshotStore.Verify(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty));
            mockEventStore.Verify(t => t.LoadAsync<FakeAggregate>(Guid.Empty, 0));

            Assert.Same(fakeAggregate, aggregate);
            Assert.Equal(fakeEvent.Value, fakeAggregate.Value);
        }


        [Fact]
        public async Task OnFindAsync_WithVersion_BuildAggregateFromEventStore()
        {
            var fakeEvent = new FakeEvent(Guid.NewGuid(), Guid.Empty, 1, null, "Version 1");
            var mockEventStore = new Mock<IEventStore>();

            mockEventStore.Setup(t => t.LoadByMaxVersionAsync<FakeAggregate>(Guid.Empty, 1)).ReturnsAsync(Enumerable.Repeat(fakeEvent, 1));

            var sut = new Repository(
                mockEventStore.Object,
                Mock.Of<ISnapshotStore>(),
                Mock.Of<IBus>(),
                Mock.Of<ILogger>()
                );

            var aggregate = await sut.GetByIdAsync<FakeAggregate>(Guid.Empty, 1);

            mockEventStore.Verify(t => t.LoadByMaxVersionAsync<FakeAggregate>(Guid.Empty, 1));
        }

        [Fact]
        public async Task OnFindAsync_WithVersion_BuildAggregateFromSnapshotStore()
        {
            var fakeAggregate = new FakeAggregate(Guid.Empty);
            var mockSnapshotStore = new Mock<ISnapshotStore>();

            mockSnapshotStore.Setup(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty)).ReturnsAsync(fakeAggregate);

            var sut = new Repository(
                Mock.Of<IEventStore>(),
                mockSnapshotStore.Object,
                Mock.Of<IBus>(),
                Mock.Of<ILogger>()
                );

            var aggregate = await sut.GetByIdAsync<FakeAggregate>(Guid.Empty, 1);

            mockSnapshotStore.Verify(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty));

            Assert.Same(fakeAggregate, aggregate);
        }

        [Fact]
        public async Task OnFindAsync_WithVersion_BuildAggregateFromEventStoreAndSnapshotStore()
        {
            var fakeAggregate = new FakeAggregate(Guid.Empty);

            var fakeEvent1 = new FakeEvent(Guid.NewGuid(), Guid.Empty, 1, null, "Version 1");
            var fakeEvent2 = new FakeEvent(Guid.NewGuid(), Guid.Empty, 2, null, "Version 2");
            var fakeEvent3 = new FakeEvent(Guid.NewGuid(), Guid.Empty, 3, null, "Version 3");

            fakeAggregate.Hydrate(new Event[] { fakeEvent1 });

            var mockEventStore = new Mock<IEventStore>();
            var mockSnapshotStore = new Mock<ISnapshotStore>();

            mockSnapshotStore.Setup(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty)).ReturnsAsync(fakeAggregate);
            mockEventStore.Setup(t => t.LoadByMaxVersionAsync<FakeAggregate>(Guid.Empty, 1, 2)).ReturnsAsync(new Event[] { fakeEvent2 });

            var sut = new Repository(
                mockEventStore.Object,
                mockSnapshotStore.Object,
                Mock.Of<IBus>(),
                Mock.Of<ILogger>()
                );


            var aggregate = await sut.GetByIdAsync<FakeAggregate>(Guid.Empty, 2);

            mockSnapshotStore.Verify(t => t.GetByIdAsync<FakeAggregate>(Guid.Empty));
            mockEventStore.Verify(t => t.LoadByMaxVersionAsync<FakeAggregate>(Guid.Empty, 1 , 2));

            Assert.Same(fakeAggregate, aggregate);
            Assert.Equal(fakeEvent2.Value, fakeAggregate.Value);
        }
    }
}
