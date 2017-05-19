using KJDevSec.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KJDevSec.Azure
{
    [Table("Commands")]
    public class CommandEntity
    {
        public CommandEntity(Command command)
        {
            this.Id = command.CommandId;
            this.CorrelationId = command.CorrelationId;
            this.ExecutedBy = command.ExecutedBy;
            this.ScheduledAt = DateTimeOffset.UtcNow;
            this.Executed = false;
            this.Success = false;
            this.Payload = Config.MessageSerializer.Serialize(command);
            this.ErrorDescription = string.Empty;
        }

        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid? ExecutedBy { get; set; }
        public DateTimeOffset ScheduledAt { get; set; }
        public DateTimeOffset? ExecutedAt { get; set; }
        public bool Executed { get; set; }
        public bool Success { get; set; }

        public byte[] Payload { get; set; }
        public string ErrorDescription { get; set; }
    }
}
