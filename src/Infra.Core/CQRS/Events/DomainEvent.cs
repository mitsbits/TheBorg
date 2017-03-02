using System;

namespace Borg.Infra.CQRS
{
    public abstract class DomainEvent<TKey> : Event, IDomainEvent<TKey> where TKey : IEquatable<TKey>
    {
        protected DomainEvent() : base()
        {
        }

        protected DomainEvent(TKey id, int? version = 0) : base()
        {
            Id = id;
            Version = version ?? 0;
        }

        public TKey Id { get; protected set; }
        public int Version { get; protected set; }

        public void SetId(TKey key)
        {
            Id = key;
        }

        public void SetVersionAndResetTimestamp(int version)
        {
            Version = version;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}