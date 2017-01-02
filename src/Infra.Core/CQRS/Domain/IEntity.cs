using System;

namespace Borg.Infra.CQRS
{
    public interface IEntity<out TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }

    public interface IEntity
    {
    }
}