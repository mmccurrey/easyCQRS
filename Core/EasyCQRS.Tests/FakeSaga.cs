using EasyCQRS.EventSourcing;
using System;
using System.Collections.Generic;
using System.Text;
using EasyCQRS.Messaging;

namespace EasyCQRS.Tests
{
    public class FakeSaga : ISaga
    {
        public FakeSaga()
        {
            this.PendingCommands = new List<Command>();
        }

        public Guid Id { get; set; }

        public Guid CorrelationId { get; set; }

        public bool Completed { get; set; }

        public ICollection<Command> PendingCommands { get; private set; }
    }
}
