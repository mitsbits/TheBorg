using Borg.Infra.Messaging;
using System;

namespace Borg.Framework.Services.Notifications
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