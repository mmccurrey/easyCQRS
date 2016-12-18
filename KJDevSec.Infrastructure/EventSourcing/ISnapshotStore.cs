using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.EventSourcing
{
    public interface ISnapshotStore
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : AggregateRoot;
        Task SaveAsync<T>(T item) where T : AggregateRoot;
    }
}
