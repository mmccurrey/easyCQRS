using EasyCQRS.Azure.Messaging;
using EasyCQRS.Diagnostics;
using EasyCQRS.Messaging;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EasyCQRS.Azure.Tests
{
    public class ServiceBusMessageSubscriberTests
    {
        [Fact]
        public void ServiceBusEventSubscriber_IsAssignableFromIEventSubscriber()
        {
            var sut = new ServiceBusMessageSubscriber(
                                    Mock.Of<IServiceProvider>(),
                                    Mock.Of<IServiceBusManagementClient>(),
                                    Mock.Of<IMessageSerializer>(),
                                    Mock.Of<IConfigurationManager>(),
                                    Mock.Of<ILogger>());

            Assert.IsAssignableFrom<IMessageSubscriber>(sut);
        }
    }
}
