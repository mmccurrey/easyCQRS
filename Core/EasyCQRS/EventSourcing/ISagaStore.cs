using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.EventSourcing
{
    public interface ISagaStore
    {
        Task<T> FindAsync<T>(Guid id) where T : ISaga;
        Task SaveAsync<T>(T saga) where T : ISaga;
    }
}
