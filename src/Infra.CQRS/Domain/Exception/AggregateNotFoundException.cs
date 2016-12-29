using System;

namespace Borg.Infra.CQRS
{
    public class AggregateNotFoundException<TKey> : Exception where TKey : IEquatable<TKey>
    {
        public AggregateNotFoundException(Type t, TKey id)
            : base($"Aggregate {id} of type {t.FullName} was not found")
        { }
    }
}