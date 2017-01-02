using System;

namespace Borg.Infra.CQRS
{
    public class EventsOutOfOrderException<TKey> : Exception where TKey : IEquatable<TKey>
    {
        public EventsOutOfOrderException(TKey id)
            : base($"Eventstore gave event for aggregate {id} out of order")
        { }
    }
}