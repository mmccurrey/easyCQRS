﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public abstract class Event : Message
    {
        public Guid EventId { get; private set; }
        public Guid CorrelationId { get; private set; }
        public Guid AggregateId { get; private set; }
        public long Version { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
        public Guid? ExecutedBy { get; private set; }

        protected Event() { }

        public Event(Guid eventId, Guid correlationId, Guid aggregateId, long version, Guid? executedBy, DateTimeOffset timestamp)
        {
            this.EventId = eventId;
            this.CorrelationId = correlationId;
            this.Timestamp = timestamp;
            this.AggregateId = aggregateId;
            this.Version = version;
            this.ExecutedBy = executedBy;
        }

        public Event(Guid correlationId, Guid aggregateId, long version, Guid? executedBy, DateTimeOffset timestamp)
            : this(Guid.NewGuid(), correlationId, aggregateId, version, executedBy, timestamp)
        {     
        }

        public Event(Guid correlationId, Guid aggregateId, long version, Guid? executedBy): 
            this(correlationId, aggregateId, version, executedBy, DateTimeOffset.UtcNow)
        {
        }

        public Event(Guid correlationId, Guid aggregateId, long version) :
            this(correlationId, aggregateId, version, null, DateTimeOffset.UtcNow)
        {
        }
    }
}
