using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{   
    [Table("Events")]
    internal class EventEntity
    {        
        [Key, Column(Order = 2)]
        public Guid AggregateId { get; set; }

        public Guid CorrelationId { get; set; }

        [Key, Column(Order = 3)]
        public long Version { get; set; }

        [Key, Column(Order = 1), MaxLength(300)]
        public string SourceType { get; set; }

        public DateTimeOffset Date { get; set; }
        public string Type { get; set; }

        public string FullName { get; set; }

        [MaxLength(Int32.MaxValue)]
        public byte[] Payload { get; set; }
    }
}
