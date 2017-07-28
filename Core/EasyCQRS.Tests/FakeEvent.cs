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

        public FakeEvent(Guid aggregateId, long version, string value)
            : base(aggregateId, version)
        {
            Value = value;
        }
    }
}
