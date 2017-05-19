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
        private readonly InfrastructureContext context;
        public EventStore(InfrastructureContext context)
        {
            this.context = context ?? throw new ArgumentNullException("context");
        }

        public async Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.context.Events.Where(e => e.AggregateId == sourceId && e.SourceType == type).ToListAsync();

            if (events != null)
            {
                return events.Select(a => a.Event);
            }

            return new List<Event>();
        }

        public async Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId, long lastKnownVersion) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.context.Events
                .Where(e => e.AggregateId == sourceId)
                .Where(e => e.SourceType == type)
                .Where(e => e.Version > lastKnownVersion)
                .ToListAsync();

            if (events != null)
            {
                return events.Select(a => a.Event);
            }

            return new List<Event>();
        }

        public async Task<IEnumerable<Event>> LoadByMaxVersionAsync<T>(Guid sourceId, long maxVersion) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.context.Events
                            .Where(e => e.AggregateId == sourceId)
                            .Where(e => e.SourceType == type)
                            .Where(e => e.Version <= maxVersion)
                            .ToListAsync();

            if (events != null)
            {
                return events.Select(a => a.Event);
            }

            return Enumerable.Empty<Event>();
        }

        public async Task<IEnumerable<Event>> LoadByMaxVersionAsync<T>(Guid sourceId, long lastKnownVersion, long maxVersion) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.context.Events
                .Where(e => e.AggregateId == sourceId)
                .Where(e => e.SourceType == type)
                .Where(e => e.Version > lastKnownVersion)
                .Where(e => e.Version <= maxVersion)
                .ToListAsync();

            if (events != null)
            {
                return events.Select(a => a.Event);
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

            context.Events.Add(new EventEntity(aggregateTypeName, @event));
        }

        public Task SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}
