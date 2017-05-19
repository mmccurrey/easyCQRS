using KJDevSec.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Azure
{   
    [Table("Events")]
    internal class EventEntity
    {
        private Event @event; 

        public EventEntity() { }
        public EventEntity(string aggregateType, Event @event)
        {
            this.AggregateId = @event.AggregateId;
            this.CorrelationId = @event.CorrelationId;
            this.Version = @event.Version;
            this.SourceType = aggregateType;
            this.Payload = Config.MessageSerializer.Serialize(@event);
            this.Date = DateTimeOffset.UtcNow;
            this.Type = @event.GetType().FullName;

            this.@event = @event;
        }

        [Key, Column(Order = 2)]
        public Guid AggregateId { get; set; }

        public Guid CorrelationId { get; set; }

        [Key, Column(Order = 3)]
        public long Version { get; set; }

        [Key, Column(Order = 1), MaxLength(300)]
        public string SourceType { get; set; }

        public DateTimeOffset Date { get; set; }
        public string Type { get; set; }

        [MaxLength(Int32.MaxValue)]
        public byte[] Payload { get; set; }

        public Event Event
        {
            get
            {
                if(@event == null)
                {
                    @event = Config.MessageSerializer.Deserialize<Event>(Payload);
                }

                return @event;
            }
        }
    }
}
