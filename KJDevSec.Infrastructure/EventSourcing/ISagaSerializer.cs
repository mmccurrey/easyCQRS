using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.EventSourcing
{
    public interface ISagaSerializer
    {
        byte[] Serialize<TSaga>(TSaga saga) where TSaga : ISaga;

        TSaga Deserialize<TSaga>(byte[] data) where TSaga : ISaga;        
    }
}
