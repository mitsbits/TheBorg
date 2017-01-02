using System;

namespace Borg.Infra.CQRS
{
    public abstract class Snapshot<TKey, T> : SnapshotBase<TKey>, ISnapshot<TKey, T> where T : IAggregateRoot<TKey> where TKey : IEquatable<TKey>
    {
        protected Snapshot()
        {
        }

        protected Snapshot(TKey id, T aggregate) : this()
        {
            Id = id;
            Version = aggregate.Version;
            Payload = aggregate;
        }

        public T Payload { get; set; }
    }
}