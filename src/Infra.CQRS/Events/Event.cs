using System;

namespace Borg.Infra.CQRS
{
    public abstract class Event : IEvent
    {
        public DateTimeOffset TimeStamp { get; protected set; } = DateTimeOffset.UtcNow;
    }
}