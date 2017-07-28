using EasyCQRS.EventSourcing;
using EasyCQRS.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.EventSourcing
{
    internal class EventStore: IEventStore
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMessageSerializer messageSerializer;
        private readonly InfrastructureContext db;

        public EventStore(
            IHttpContextAccessor httpContextAccessor,
            IMessageSerializer messageSerializer,            
            InfrastructureContext db)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.messageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.db = db ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId) where T : AggregateRoot
        {
            var type = typeof(T).FullName;
            var events = await this.db.Events.Where(e => e.AggregateId == sourceId && e.SourceType == type).ToListAsync();

            if (events != null)
            {
                return events.Select(a => GetEvent(a));
            }

            return new List<Event>();
        }

        public async Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId, long lastKnownVersion) where T : AggregateRoot
        {
            var type = typeof(T).FullName;
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
            var type = typeof(T).FullName;
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
            var type = typeof(T).FullName;
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
            var aggregateTypeName = aggregateType.FullName;
            if (aggregateTypeName.Length > 800)
            {
                throw new InvalidOperationException("Cannot save an event when the aggregate type name length is greather than 800.");
            }

            var entity = new EventEntity
            {
                AggregateId = @event.AggregateId,
                CorrelationId = httpContextAccessor?.HttpContext?.TraceIdentifier,                
                Version = @event.Version,
                Date = DateTimeOffset.UtcNow,
                SourceType = aggregateTypeName,
                Type = @event.GetType().AssemblyQualifiedName,
                FullName = @event.GetType().FullName,
                ExecutedBy = httpContextAccessor?.HttpContext?.User.GetId(),
                Payload = messageSerializer.Serialize(@event)
            };

            db.Events.Add(entity);
        }

        public Task SaveChangesAsync()
        {
            db.SaveChanges();

            return Task.FromResult(true);
        }

        private Event GetEvent(EventEntity entity)
        {
            return messageSerializer.Deserialize<Event>(Type.GetType(entity.Type), entity.Payload);
        }
    }
}
