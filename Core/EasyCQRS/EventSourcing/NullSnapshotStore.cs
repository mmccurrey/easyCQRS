using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.EventSourcing
{
    internal class NullSnapshotStore : ISnapshotStore
    {

        public Task<T> GetByIdAsync<T>(Guid id) where T : AggregateRoot
        {
            return Task.FromResult<T>(default(T));
        }

        public Task<T> GetByIdAsync<T>(Guid id, long maxVersion) where T : AggregateRoot
        {
            return Task.FromResult<T>(default(T));
        }

        public Task SaveAsync<T>(T item) where T : AggregateRoot
        {
            return Task.FromResult(0);
        }
    }
}
