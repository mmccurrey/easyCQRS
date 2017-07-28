using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public abstract class Event : Message
    {
        public Guid AggregateId { get; private set; }
        public long Version { get; private set; }

        protected Event() { }

        public Event(Guid aggregateId, long version):
            base()
        {
            this.AggregateId = aggregateId;
            this.Version = version;
        }
    }
}
