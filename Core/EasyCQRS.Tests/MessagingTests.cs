using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace EasyCQRS.Tests
{
    public class MessagingTests
    {
        [Fact]
        public void Command_IsAssignableFromIMessage()
        {
            Assert.True(typeof(IMessage).GetTypeInfo().IsAssignableFrom(typeof(Command)));
        }

        [Fact]
        public void Event_IsAssignableFromIMessage()
        {
            Assert.True(typeof(IMessage).GetTypeInfo().IsAssignableFrom(typeof(Event)));
        }
    }
}
