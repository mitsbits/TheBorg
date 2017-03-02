using Borg.Framework.Services.Notifications;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;
using System.Threading.Tasks;

namespace Borg.Framework.Services.Notifications
{
    public interface IUserNotificationService
    {
        Task Add(string recipientIdentifier, ResponseStatus responseStatus, string title, string message);

        Task<IPagedResult<IUserNotification>> Find(string recipientIdentifier, int page, int rows);

        Task<IPagedResult<IUserNotification>> Pending(string recipientIdentifier, int page, int rows);

        Task<IUserNotification> Get(string notificationIdentifier);

        Task Acknowledge(string notificationIdentifier);

        Task Dismiss(string notificationIdentifier);
    }
}

namespace Borg
{
    public static class INotificationServiceExtensions
    {
        public static async Task Info(this IUserNotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Info, title, message).AnyContext();
        }

        public static async Task Warning(this IUserNotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Warning, title, message).AnyContext();
        }

        public static async Task Error(this IUserNotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Error, title, message).AnyContext();
        }

        public static async Task Success(this IUserNotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Success, title, message).AnyContext();
        }

        private static Task Notify(this IUserNotificationService service, string recipientIdentifier, ResponseStatus responseStatus, string title, string message)
        {
            return service.Add(recipientIdentifier, responseStatus, title, message);
        }
    }
}