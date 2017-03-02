using Borg.Infra.CQRS;
using System;

namespace Borg.Framework.Services.Notifications
{
    public class NotificationCreatedEvent : IEvent
    {
        public NotificationCreatedEvent(IUserNotification userNotification)
        {
            UserNotification = userNotification;
        }

        public IUserNotification UserNotification { get; }
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    }
}