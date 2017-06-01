using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{
    [Table("Snapshots")]
    public class SnapshotEntity
    {
        [Key, Column(Order = 1), MaxLength(300)]
        public string SourceType { get; set; } 

        [Key, Column(Order = 2)]
        public Guid AggregateId { get; set; }

        [Key, Column(Order = 3)]
        public long Version { get; set; }

        [MaxLength(Int32.MaxValue)]
        public byte[] Payload { get; set; }
    }
}
