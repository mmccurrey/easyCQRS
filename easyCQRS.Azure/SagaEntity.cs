using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{
    [Table("Sagas")]
    internal class SagaEntity
    {
        [Key, Column(Order = 1)]
        public Guid Id { get; set; }

        public Guid CorrelationId { get; set; }

        [Key, Column(Order = 2), StringLength(500)]
        public string Type { get; set; }

        public bool Completed { get; set; }

        public byte[] Payload { get; set; }
    }
}