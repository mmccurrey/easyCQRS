using KJDevSec.EventSourcing;
using KJDevSec.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KJDevSec.Azure.EventSourcing
{
    class SQLEventStore: IEventStore
    {
        private readonly SQLEventSourcingContext context;
        public SQLEventStore(SQLEventSourcingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
        }

        public IEnumerable<Event> Load()
        {
            return this.context.Events.ToList().Select(e => e.GetUnderlyingEvent());
        }

        public async Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId) where T : AggregateRoot
        {
            var type = typeof(T).Name;
            var events = await this.context.Events.Where(e => e.AggregateId == sourceId && e.SourceType == type).ToListAsync();

            if (events != null)
            {
                return events.Select(a => a.GetUnderlyingEvent());
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
                return events.Select(a => a.GetUnderlyingEvent());
            }

            return new List<Event>();
        }

        public void Save(Type aggregateType, Event @event)
        {
            var aggregateTypeName = aggregateType.Name;
            if (aggregateTypeName.Length > 800)
            {
                throw new InvalidOperationException("Cannot save an event when the aggregate type name length is greather than 800.");
            }

            var sqlEvent = new SQLEvent(aggregateTypeName, @event);

            context.Events.Add(sqlEvent);
        }

        public Task SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}
