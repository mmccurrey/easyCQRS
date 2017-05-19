using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.EventSourcing
{
    public interface ISaga
    {
        Guid Id { get; }
        Guid CorrelationId { get; }
        bool Completed { get; }
        ICollection<Command> PendingCommands { get; }
    }
}
