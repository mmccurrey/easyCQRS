using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public interface IAggregateSerializer
    {
        byte[] Serialize<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : AggregateRoot;

        TAggregateRoot Deserialize<TAggregateRoot>(byte[] data) where TAggregateRoot : AggregateRoot;
    }
}
