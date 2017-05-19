using KJDevSec.EventSourcing;
using KJDevSec.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KJDevSec.Azure.EventSourcing
{
    class EventStore: IEventStore
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly InfrastructureContext db;

        public EventStore(
            IMessageSerializer messageSerializer,
            InfrastructureContext db)
        {
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException("messageSerializer");
            this.db = db ?? throw new ArgumentNullException("db");
        }

        public async Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.db.Events.Where(e => e.AggregateId == sourceId && e.SourceType == type).ToListAsync();

            if (events != null)
            {
                return events.Select(a => GetEvent(a));
            }

            return new List<Event>();
        }

        public async Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId, long lastKnownVersion) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.db.Events
                .Where(e => e.AggregateId == sourceId)
                .Where(e => e.SourceType == type)
                .Where(e => e.Version > lastKnownVersion)
                .ToListAsync();

            if (events != null)
            {
                return events.Select(a => GetEvent(a));
            }

            return new List<Event>();
        }

        public async Task<IEnumerable<Event>> LoadByMaxVersionAsync<T>(Guid sourceId, long maxVersion) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.db.Events
                            .Where(e => e.AggregateId == sourceId)
                            .Where(e => e.SourceType == type)
                            .Where(e => e.Version <= maxVersion)
                            .ToListAsync();

            if (events != null)
            {
                return events.Select(a => GetEvent(a));
            }

            return Enumerable.Empty<Event>();
        }

        public async Task<IEnumerable<Event>> LoadByMaxVersionAsync<T>(Guid sourceId, long lastKnownVersion, long maxVersion) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.db.Events
                .Where(e => e.AggregateId == sourceId)
                .Where(e => e.SourceType == type)
                .Where(e => e.Version > lastKnownVersion)
                .Where(e => e.Version <= maxVersion)
                .ToListAsync();

            if (events != null)
            {
                return events.Select(a => GetEvent(a));
            }

            return Enumerable.Empty<Event>();
        }

        public void Save(Type aggregateType, Event @event)
        {
            var aggregateTypeName = aggregateType.Name;
            if (aggregateTypeName.Length > 800)
            {
                throw new InvalidOperationException("Cannot save an event when the aggregate type name length is greather than 800.");
            }

            var entity = new EventEntity
            {
                AggregateId = @event.EventId,
                CorrelationId = @event.CorrelationId,
                Version = @event.Version,
                Date = @event.Timestamp,
                SourceType = aggregateTypeName,
                Type = @event.GetType().FullName,
                Payload = messageSerializer.Serialize(@event)
            };

            db.Events.Add(entity);
        }

        public Task SaveChangesAsync()
        {
            return db.SaveChangesAsync();
        }

        private Event GetEvent(EventEntity entity)
        {
            return messageSerializer.Deserialize<Event>(entity.Payload);
        }
    }
}
