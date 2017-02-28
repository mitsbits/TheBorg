using System;
using Borg.Infra.Messaging;

namespace Borg.Framework.Services.Notifications
{
    public class Notification : INotification
    {
        public string Message { get;  set; }
        public string NotificationIdentifier { get; protected set; } = Guid.NewGuid().ToString();

        public string RecipientIdentifier { get;  set; }

        public ResponseStatus ResponseStatus { get;  set; }

        public DateTimeOffset Timestamp { get;  } = DateTimeOffset.UtcNow;

        public string Title { get;  set; }
    }
}