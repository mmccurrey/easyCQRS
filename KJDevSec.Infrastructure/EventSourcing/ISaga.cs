using KJDevSec.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.EventSourcing
{
    public interface ISaga
    {
        Guid Id { get; }
        Guid CorrelationId { get; }
        bool Completed { get; }
        ICollection<Command> PendingCommands { get; }
    }
}
