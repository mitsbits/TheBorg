using System;

namespace Borg.Infra.CQRS
{
    public interface ISnapshot<out TKey, out T> where TKey : IEquatable<TKey> where T : IAggregateRoot<TKey>
    {
        T Payload { get; }
        TKey Id { get; }
        int Version { get; }
    }
}