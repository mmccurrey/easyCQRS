using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyCQRS.Tests
{
    class FakeIntegrationEvent : IntegrationEvent
    {
        public string Value { get; protected set; }

        protected FakeIntegrationEvent()
            : base() { }

        public FakeIntegrationEvent(Guid correlationId, Guid? executedBy, string value)
            : base(correlationId, executedBy)
        {
            Value = value;
        }
    }
}
