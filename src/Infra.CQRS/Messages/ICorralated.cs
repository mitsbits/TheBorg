using System;

namespace Borg.Infra.CQRS
{
    public interface ICorralated<out TKey> where TKey : IEquatable<TKey>
    {
        TKey CorralationId { get; }
    }
}