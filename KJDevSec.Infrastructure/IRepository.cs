using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public interface IRepository : IUnitOfWork
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : AggregateRoot;
        void Save<T>(T aggregate) where T : AggregateRoot;
    }
}
