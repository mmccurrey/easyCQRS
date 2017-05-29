using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Tests
{
    public class FakeAggregate : AggregateRoot
    {
        public string Value { get; private set; }

        public FakeAggregate(Guid id) : base(id)
        {
        }

        public void Fire(string value)
        {
            ApplyChange(new FakeEvent(Guid.NewGuid(), this.Id, this.Version + 1, null, value));
        }

        protected void Apply(FakeEvent @event)
        {
            Value = @event.Value;
        }
    }
}
