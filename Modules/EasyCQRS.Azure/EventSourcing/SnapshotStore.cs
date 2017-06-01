using EasyCQRS.EventSourcing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure.EventSourcing
{
    internal class SnapshotStore : ISnapshotStore
    {
        private readonly IAggregateSerializer aggregateSerializer;
        private readonly InfrastructureContext db;

        public SnapshotStore(IAggregateSerializer aggregateSerializer, InfrastructureContext db)
        {
            this.aggregateSerializer = aggregateSerializer ?? throw new ArgumentNullException("aggregateSerializer");
            this.db = db ?? throw new ArgumentNullException("db");
        }

        public async Task<T> GetByIdAsync<T>(Guid id) where T : AggregateRoot
        {
            var aggregateType = typeof(T).FullName;
            var snapshotEntity = await db.Snapshots
                                           .Where(s => s.AggregateId == id)
                                           .Where(s => s.SourceType == aggregateType)
                                           .OrderByDescending(s => s.Version)
                                           .FirstOrDefaultAsync();

            if(snapshotEntity != null)
            {
                return aggregateSerializer.Deserialize<T>(snapshotEntity.Payload);
            }

            return default(T);
        }

        public async Task<T> GetByIdAsync<T>(Guid id, long maxVersion) where T : AggregateRoot
        {
            var aggregateType = typeof(T).FullName;
            var snapshotEntity = await db.Snapshots
                                           .Where(s => s.AggregateId == id)
                                           .Where(s => s.SourceType == aggregateType)
                                           .Where(s => s.Version <= maxVersion)
                                           .OrderByDescending(s => s.Version)
                                           .FirstOrDefaultAsync();

            if (snapshotEntity != null)
            {
                return aggregateSerializer.Deserialize<T>(snapshotEntity.Payload);
            }

            return default(T);
        }

        public Task SaveAsync<T>(T item) where T : AggregateRoot
        {
            var entity = new SnapshotEntity
            {
                AggregateId = item.Id,
                SourceType = item.GetType().FullName,
                Version = item.Version,
                Payload = aggregateSerializer.Serialize(item)
            };

            db.Snapshots.Add(entity);

            db.SaveChanges();

            return Task.FromResult(true);            
        }
    }
}
