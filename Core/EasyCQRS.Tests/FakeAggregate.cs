using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Tests
{
    public class FakeAggregate : AggregateRoot
    {

        protected FakeAggregate(): base() { }

        public FakeAggregate(Guid id) : base(id)
        {
        }

        public FakeAggregate(Guid id, long version):
            this(id)
        {
            this.Version = version;
        }

        public string Value { get; protected set; }

        public void Fire(string value)
        {
            ApplyChange(new FakeEvent(this.Id, this.Version + 1, value));
        }

        protected void Apply(FakeEvent @event)
        {
            Value = @event.Value;
        }
    }
}
