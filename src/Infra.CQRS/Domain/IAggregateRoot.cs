using System;

namespace Borg.Infra.CQRS
{
    public interface IAggregateRoot<out TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        int Version { get; }
    }
}