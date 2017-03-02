using System;

namespace Borg.Infra.CQRS
{
    public abstract class Event : IEvent
    {
        public DateTimeOffset Timestamp { get; protected set; } = DateTimeOffset.UtcNow;
    }
}