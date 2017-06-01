using EasyCQRS.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EasyCQRS.Tests
{
    public class JsonAggregateSerializerTests
    {
        [Fact]
        public void JsonAggregateSerializer_IsAssignableFromIAggregateSerializer()
        {
            Assert.IsAssignableFrom<IAggregateSerializer>(new JsonAggregateSerializer());
        }

        [Fact]
        public void JsonAggregateSerializer_SerializesAggregateProperly()
        {
            var sut = new JsonAggregateSerializer();
            var aggregate = new FakeAggregate(Guid.Empty);

            var bytes = sut.Serialize(aggregate);

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void JsonAggregateSerializer_DeserializesAggregateProperty()
        {
            var aggregateId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var version = 20L;

            var sut = new JsonAggregateSerializer();
            var bytes = sut.Serialize(new FakeAggregate(aggregateId, version));
            var aggregate = sut.Deserialize<FakeAggregate>(bytes);

            Assert.Equal(aggregateId, aggregate.Id);
            Assert.Equal(version, aggregate.Version);
        }
    }
}
