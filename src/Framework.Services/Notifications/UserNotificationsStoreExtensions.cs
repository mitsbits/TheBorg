using System.Threading.Tasks;
using Borg.Framework.Services.Notifications;
using Borg.Infra.Messaging;

namespace Borg
{
    public static class UserNotificationsStoreExtensions
    {
        public static async Task Info(this IUserNotificationsStore service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Info, title, message).AnyContext();
        }

        public static async Task Warning(this IUserNotificationsStore service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Warning, title, message).AnyContext();
        }

        public static async Task Error(this IUserNotificationsStore service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Error, title, message).AnyContext();
        }

        public static async Task Success(this IUserNotificationsStore service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Success, title, message).AnyContext();
        }

        private static Task Notify(this IUserNotificationsStore service, string recipientIdentifier, ResponseStatus responseStatus, string title, string message)
        {
            return service.Add(recipientIdentifier, responseStatus, title, message);
        }
    }
}