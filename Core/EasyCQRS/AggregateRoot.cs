using EasyCQRS.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCQRS
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> changes = new List<Event>();

        protected AggregateRoot(): base() { }

        public AggregateRoot(Guid id)
        {
            this.Id = id;
            this.Version = 0;
        }

        public Guid Id { get; protected set; }

        public long Version { get; protected set; }

        #region Equals & GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == typeof(AggregateRoot) && Equals((AggregateRoot)obj);
        }

        public bool Equals(AggregateRoot other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || other.Id.Equals(Id);
        }
        #endregion

        #region Event Sourcing

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return this.changes;
        }

        public void MarkChangesAsCommitted()
        {
            this.changes.Clear();
        }

        public void Hydrate(IEnumerable<Event> pastEvents)
        {
            foreach (var @event in pastEvents)
            {
                ApplyChange(@event, false);
            }
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }


        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            this.Version = @event.Version;
            if (isNew) changes.Add(@event);
        }
        #endregion
    }
}
