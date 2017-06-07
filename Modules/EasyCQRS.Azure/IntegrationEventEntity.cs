﻿using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS.Azure
{   
    [Table("IntegrationEvents")]
    internal class IntegrationEventEntity
    {        
        [Key, Column(Order = 1)]
        public Guid Id { get; set; }

        public Guid CorrelationId { get; set; }

        public DateTimeOffset Date { get; set; }

        public Guid? ExecutedBy { get; set; }

        public string Type { get; set; }

        public string FullName { get; set; }

        [MaxLength(Int32.MaxValue)]
        public byte[] Payload { get; set; }
    }
}
