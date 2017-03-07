using System;
using Borg.Infra.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Borg.Framework.UserNotifications
{
    internal class UserNotification : IUserNotification
    {
        public string Message { get; set; }
        public string NotificationIdentifier { get; protected set; } = Guid.NewGuid().ToString();

        public string RecipientIdentifier { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseStatus ResponseStatus { get; set; }

        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

        public string Title { get; set; }
        public bool Acknowledged { get; set; } = false;
    }
}