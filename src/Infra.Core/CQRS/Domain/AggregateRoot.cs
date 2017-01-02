using System;
using System.Collections.Generic;

namespace Borg.Infra.CQRS
{
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey> where TKey : IEquatable<TKey>
    {
        private readonly List<IDomainEvent<TKey>> _changes = new List<IDomainEvent<TKey>>();

        public int Version { get; protected set; }

        public IEnumerable<IDomainEvent<TKey>> GetUncommittedChanges()
        {
            lock (_changes)
            {
                return _changes.ToArray();
            }
        }

        public IEnumerable<IDomainEvent<TKey>> FlushUncommitedChanges()
        {
            lock (_changes)
            {
                var changes = _changes.ToArray();
                var i = 0;
                foreach (var @event in changes)
                {
                    if (@event.Id == null)
                    {
                        throw new AggregateOrEventMissingIdException(GetType(), @event.GetType());
                    }
                    if (@event.Id.Equals(default(TKey)))
                    {
                        @event.SetId(Id);
                    }
                    i++;
                    @event.SetVersionAndResetTimestamp(Version + i);
                }
                Version = Version + _changes.Count;
                _changes.Clear();
                return changes;
            }
        }

        public void LoadFromHistory(IEnumerable<IDomainEvent<TKey>> history)
        {
            foreach (var e in history)
            {
                if (e.Version != Version + 1)
                {
                    throw new EventsOutOfOrderException<TKey>(e.Id);
                }
                ApplyChange(e, false);
            }
        }

        protected void ApplyChange(IDomainEvent<TKey> @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(IDomainEvent<TKey> @event, bool isNew)
        {
            lock (_changes)
            {
                this.AsDynamic().Apply(@event);
                if (isNew)
                {
                    _changes.Add(@event);
                }
                else
                {
                    SetId(@event.Id);
                    Version++;
                }
            }
        }
    }
}