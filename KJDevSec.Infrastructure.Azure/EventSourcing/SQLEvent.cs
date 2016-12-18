using KJDevSec.Messaging;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace KJDevSec.Azure.EventSourcing
{
    class SQLEvent
    {
        public SQLEvent() { }
        public SQLEvent(string aggregateType, Event @event)
        {
            this.AggregateId = @event.AggregateId;
            this.Version = @event.Version;
            this.SourceType = aggregateType;
            this.Payload = SerializeEvent(@event);
            this.Date = DateTimeOffset.UtcNow;
        }

        [Key, Column(Order = 2)]
        public Guid AggregateId { get; set; }

        [Key, Column(Order = 3)]
        public long Version { get; set; }

        [Key, Column(Order = 1), MaxLength(300)]
        public string SourceType { get; set; }

        public DateTimeOffset Date { get; set; }
        public string Type { get; set; }

        [MaxLength(Int32.MaxValue)]
        public byte[] Payload { get; set; }

        public Event GetUnderlyingEvent()
        {
            using (var oStream = new MemoryStream(Payload))
            {
                var formatter = new XmlSerializer(System.Type.GetType(this.Type));
                return (Event)formatter.Deserialize(oStream);
            }
        }

        private static byte[] SerializeEvent(Event @event)
        {
            using (var oStream = new MemoryStream())
            {
                var formatter = new XmlSerializer(@event.GetType());

                formatter.Serialize(oStream, @event);

                return oStream.ToArray();
            }
        }

        public static implicit operator Event(SQLEvent azureEvent)
        {
            return azureEvent.GetUnderlyingEvent();
        }
    }
}
