using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Messaging
{
    public abstract class Command : IMessage
    {
        public Guid CommandId { get; protected set; }
        public Guid? UserId { get; set; }
        public Guid? PreviousCommandId { get; set; }
        public DateTimeOffset Date { get; protected set; }

        public Command()
        {
            this.CommandId = Guid.NewGuid();
            this.Date = DateTimeOffset.UtcNow;
        }
    }
}
