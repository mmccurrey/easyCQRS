using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace EasyCQRS.Tests
{
    /*
    public class MemoryBusTests
    {
        [Fact]
        public void OnSendCommandAsync_InvokesResolveAllOnContainer()
        {
            var mockContainer = new Mock<IServiceProvider>();
            var fakeCommand = new FakeCommand();

            mockContainer.Setup(s => s.GetService(typeof(IHandler<FakeCommand>))).Returns(Enumerable.Repeat(Mock.Of<IHandler<FakeCommand>>(), 1));
            
            var sut = new MemoryBus(
                mockContainer.Object,
                Mock.Of<ILogger>()
                );

            sut.SendCommandAsync(fakeCommand);

            mockContainer.Verify(s => s.GetService(typeof(IHandler<FakeCommand>)));
        }

        [Fact]
        public void OnPublishEventsAsync_InvokesResolveAllOnContainer()
        {
            var mockContainer = new Mock<IServiceProvider>();
            var fakeEvent = new FakeEvent(Guid.NewGuid(), Guid.Empty, 1, null, "Fake Value");

            mockContainer.Setup(s => s.GetService(typeof(IHandler<FakeEvent>)))
                .Returns(Enumerable.Repeat(Mock.Of<IHandler<FakeEvent>>(), 1));

            var sut = new MemoryBus(
                mockContainer.Object,
                Mock.Of<ILogger>()
                );

            sut.PublishEventsAsync(fakeEvent);

            mockContainer.Verify(s => s.GetService(typeof(IHandler<FakeEvent>)));
        }
    }
    */
}
