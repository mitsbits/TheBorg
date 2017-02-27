using System;
using Borg.Infra.CQRS;
using Borg.Infra.Storage;

namespace Borg.Framework.Media.Events
{
    public class FileAddedEvent : IEvent
    {
        public FileAddedEvent(IFileSpec file)
        {
            File = file;
        }
        public IFileSpec File { get; }
        public DateTimeOffset TimeStamp { get; } = DateTimeOffset.UtcNow;
    }
}