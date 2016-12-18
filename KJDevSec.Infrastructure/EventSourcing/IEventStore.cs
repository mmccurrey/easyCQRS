using KJDevSec.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.EventSourcing
{
    public interface IEventStore : IUnitOfWork
    {
        Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId) where T : AggregateRoot;
        Task<IEnumerable<Event>> LoadAsync<T>(Guid sourceId, long lastKnownVersion) where T : AggregateRoot;

        void Save(Type aggregateType, Event @event);
        IEnumerable<Event> Load();
    }
}
