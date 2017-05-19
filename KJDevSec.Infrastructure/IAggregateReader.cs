using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public interface IAggregateReader
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : AggregateRoot;
        Task<T> GetByIdAsync<T>(Guid id, long version) where T : AggregateRoot;
    }
}
