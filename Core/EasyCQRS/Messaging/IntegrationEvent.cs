using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public class IntegrationEvent: Message
    {
        public Guid EventId { get; private set; }
        public Guid CorrelationId { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
        public Guid? ExecutedBy { get; private set; }

        protected IntegrationEvent() { }

        public IntegrationEvent(Guid eventId, Guid correlationId, Guid? executedBy, DateTimeOffset timestamp)
        {
            this.EventId = eventId;
            this.CorrelationId = correlationId;
            this.Timestamp = timestamp;
            this.ExecutedBy = executedBy;
        }

        public IntegrationEvent(Guid correlationId, Guid? executedBy, DateTimeOffset timestamp)
            : this(Guid.NewGuid(), correlationId, executedBy, timestamp)
        {
        }

        public IntegrationEvent(Guid correlationId, Guid? executedBy): 
            this(correlationId, executedBy, DateTimeOffset.UtcNow)
        {
        }

        public IntegrationEvent(Guid correlationId) :
            this(correlationId, null, DateTimeOffset.UtcNow)
        {
        }
    }
}
