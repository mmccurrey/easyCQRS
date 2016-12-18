using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Messaging
{
    public abstract class Event : IMessage
    {
        public Guid EventId { get; protected set; }
        public Guid AggregateId { get; protected set; }
        public long Version { get; protected set; }
        public DateTimeOffset Timestamp { get; protected set; }
        public Guid? UserId { get; protected set; }

        protected Event() { }
        public Event(Guid aggregateId, long version, Guid? userId, DateTimeOffset timestamp)
        {            
            this.EventId = Guid.NewGuid();
            this.Timestamp = timestamp;
            this.AggregateId = aggregateId;
            this.Version = version;
            this.UserId = userId;
        }

        public Event(Guid aggregateId, long version, Guid? userId): 
            this(aggregateId, version, userId, DateTimeOffset.UtcNow)
        {
        }

        public Event(Guid aggregateId, long version) :
            this(aggregateId, version, null, DateTimeOffset.UtcNow)
        {
        }
    }
}
