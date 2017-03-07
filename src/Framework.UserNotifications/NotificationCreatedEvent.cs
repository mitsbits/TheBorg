using System;
using Borg.Infra.CQRS;

namespace Borg.Framework.UserNotifications
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