using System;

namespace Borg.Infra.CQRS
{
    public interface IEvent : IMessage
    {
        DateTimeOffset TimeStamp { get; }
    }
}