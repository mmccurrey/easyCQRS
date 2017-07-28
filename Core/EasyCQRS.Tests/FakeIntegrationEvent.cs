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

        public FakeIntegrationEvent(string value)
            : base()
        {
            Value = value;
        }
    }
}
