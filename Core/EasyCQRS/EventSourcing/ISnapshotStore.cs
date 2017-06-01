using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.EventSourcing
{
    public interface ISnapshotStore
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : AggregateRoot;
        Task<T> GetByIdAsync<T>(Guid id, long maxVersion) where T : AggregateRoot;
        Task SaveAsync<T>(T item) where T : AggregateRoot;
    }
}
