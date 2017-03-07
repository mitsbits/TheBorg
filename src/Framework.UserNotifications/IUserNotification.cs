using System;
using Borg.Infra.Messaging;

namespace Borg.Framework.UserNotifications
{
    public interface IUserNotification
    {
        string NotificationIdentifier { get; }
        string RecipientIdentifier { get; }
        string Title { get; }
        string Message { get; }
        ResponseStatus ResponseStatus { get; }
        DateTimeOffset Timestamp { get; }
    }
}