using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Tests
{
    public class FakeEvent : Event
    {
        public string Value { get; protected set; }

        protected FakeEvent()
            : base() { }

        public FakeEvent(Guid correlationId, Guid aggregateId, long version, Guid? executedBy, string value)
            : base(correlationId, aggregateId, version, executedBy)
        {
            Value = value;
        }
    }
}
