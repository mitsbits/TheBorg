using System;

namespace Borg.Infra.CQRS
{
    public class ConcurrencyException<TKey> : Exception where TKey : IEquatable<TKey>
    {
        public ConcurrencyException(TKey id)
            : base($"A different version than expected was found in aggregate {id}")
        { }
    }
}