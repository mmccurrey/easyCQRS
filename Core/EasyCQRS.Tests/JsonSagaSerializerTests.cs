using EasyCQRS.EventSourcing;
using System;
using Xunit;

namespace EasyCQRS.Tests
{
    public class JsonSagaSerializerTests
    {
        [Fact]
        public void JsonSagaSerializer_IsAssignableFromISagaSerializer()
        {
            Assert.IsAssignableFrom<ISagaSerializer>(new JsonSagaSerializer());
        }

        [Fact]
        public void JsonSagaSerializer_SerializesSagaProperly()
        {
            var sut = new JsonSagaSerializer();
            var saga = new FakeSaga { Id = Guid.Empty };

            var bytes = sut.Serialize(saga);

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void JsonSagaSerializer_DeserializesSagaProperty()
        {
            var sagaId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();

            var sut = new JsonSagaSerializer();
            var bytes = sut.Serialize(new FakeSaga { Id = sagaId, CorrelationId = correlationId, PendingCommands = { new FakeCommand() } });
            var saga = sut.Deserialize<FakeSaga>(bytes);

            Assert.Equal(sagaId, saga.Id);
            Assert.Equal(correlationId, saga.CorrelationId);
            Assert.NotNull(saga.PendingCommands);
            Assert.Equal(1, saga.PendingCommands.Count);            
        }
    }
}
