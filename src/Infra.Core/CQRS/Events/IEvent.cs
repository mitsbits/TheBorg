using System;

namespace Borg.Infra.CQRS
{
    public interface IEvent
    {
        DateTimeOffset Timestamp { get; }
    }
}