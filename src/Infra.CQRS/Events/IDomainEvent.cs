using System;

namespace Borg.Infra.CQRS
{
    public interface IDomainEvent<TKey> : IEvent where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
        int Version { get; }

        void SetId(TKey key);

        void SetVersionAndResetTimestamp(int version);
    }
}