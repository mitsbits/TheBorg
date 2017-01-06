using System;

namespace Borg.Infra.CQRS
{
    public interface IEntity<out TKey> : IEntity where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }

    public interface IEntity
    {
    }
}