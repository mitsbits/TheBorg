using System.Threading.Tasks;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;

namespace Borg.Framework.UserNotifications
{
    public interface IUserNotificationsStore
    {
        Task Add(string recipientIdentifier, ResponseStatus responseStatus, string title, string message);

        Task<IPagedResult<IUserNotification>> Find(string recipientIdentifier, int page, int rows);

        Task<IPagedResult<IUserNotification>> Pending(string recipientIdentifier, int page, int rows);

        Task<IUserNotification> Get(string notificationIdentifier);

        Task Acknowledge(string notificationIdentifier);

        Task Dismiss(string notificationIdentifier);
    }
}

