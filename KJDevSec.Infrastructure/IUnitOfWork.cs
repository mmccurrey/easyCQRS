using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
