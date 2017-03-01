using Borg.Framework.Services.Notifications;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;
using System.Threading.Tasks;

namespace Borg.Framework.Services.Notifications
{
    public interface INotificationService
    {
        Task Add(INotification notification);

        Task<IPagedResult<INotification>> Find(string recipientIdentifier, int page, int rows);

        Task<IPagedResult<INotification>> Pending(string recipientIdentifier, int page, int rows);

        Task<INotification> Get(string notificationIdentifier);

        Task Acknowledge(string notificationIdentifier);

        Task Dismiss(string notificationIdentifier);
    }
}

namespace Borg
{
    public static class INotificationServiceExtensions
    {
        public static async Task Info(this INotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Info, title, message).AnyContext();
        }

        public static async Task Warning(this INotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Warning, title, message).AnyContext();
        }

        public static async Task Error(this INotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Error, title, message).AnyContext();
        }

        public static async Task Success(this INotificationService service, string recipientIdentifier, string title, string message)
        {
            await service.Notify(recipientIdentifier, ResponseStatus.Success, title, message).AnyContext();
        }

        private static Task Notify(this INotificationService service, string recipientIdentifier, ResponseStatus responseStatus, string title, string message)
        {
            return service.Add(new Notification()
            {
                Title = title,
                RecipientIdentifier = recipientIdentifier,
                Message = message,
                ResponseStatus = responseStatus
            });
        }
    }
}