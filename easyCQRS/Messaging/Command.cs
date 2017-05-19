using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Messaging
{
    public abstract class Command : IMessage
    {
        public Guid CommandId { get; protected set; }
        public Guid CorrelationId { get; set; }
        public Guid? ExecutedBy { get; set; }
        public Guid? PreviousCommandId { get; set; }
        public DateTimeOffset Date { get; protected set; }

        public Command()
        {
            this.CommandId = Guid.NewGuid();
            this.Date = DateTimeOffset.UtcNow;
        }
    }
}
