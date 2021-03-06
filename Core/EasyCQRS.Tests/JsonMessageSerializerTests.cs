﻿using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EasyCQRS.Tests
{
    public class JsonMessageSerializerTests
    {
        [Fact]
        public void JsonMessageSerializer_IsAssignableFromIMessageSerializer()
        {
            Assert.IsAssignableFrom<IMessageSerializer>(new JsonMessageSerializer());
        }

        [Fact]
        public void JsonMessageSerializer_SerializesEventProperly()
        {
            var sut = new JsonMessageSerializer();
            var value = Guid.NewGuid().ToString();
            var aggregateId = Guid.NewGuid();
            var @event = new FakeEvent(aggregateId, 1, value);

            var bytes = sut.Serialize(@event);

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void JsonMessageSerializer_DeserializesEventProperty()
        {
            var aggregateId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();            

            var sut = new JsonMessageSerializer();
            var bytes = sut.Serialize(new FakeEvent(aggregateId, 1, "Fake Value"));
            
            var message = (FakeEvent) sut.Deserialize<Event>(typeof(FakeEvent), bytes);
            
            Assert.Equal(aggregateId, message.AggregateId);
            Assert.Equal(1, message.Version);
            Assert.Equal("Fake Value", message.Value);
        }

    }
}
