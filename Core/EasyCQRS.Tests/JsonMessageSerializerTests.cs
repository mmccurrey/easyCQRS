using EasyCQRS.Messaging;
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
            var @event = new FakeEvent(Guid.NewGuid(), aggregateId, 1, null, value);

            var bytes = sut.Serialize(@event);

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void JsonMessageSerializer_DeserializesEventProperty()
        {
            var sut = new JsonMessageSerializer();
            var json = @"{
                          ""EventId"": ""00000000-0000-0000-0000-000000000001"",
                          ""AggregateId"": ""00000000-0000-0000-0000-000000000001"",
                          ""CorrelationId"": ""00000000-0000-0000-0000-000000000001"",
                          ""Version"": 1,
                          ""Timestamp"": ""2017-01-01T00:00:00+00:00"",
                          ""ExecutedBy"": null,
                          ""Value"": ""Fake Value""
                        }";

            var bytes = Encoding.UTF8.GetBytes(json);

            var message = sut.Deserialize<FakeEvent>(bytes);

            Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), message.AggregateId);
            Assert.Equal(1, message.Version);
            Assert.Equal("Fake Value", message.Value);
        }

    }
}
