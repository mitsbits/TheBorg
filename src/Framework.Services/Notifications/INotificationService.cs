using Borg.Infra.Relational;
using System.Threading.Tasks;

namespace Borg.Framework.Services.Notifications
{
    public interface INotificationService
    {
        Task Add(INotification notification);
        Task<IPagedResult<INotification>> Find(string recipientIdentifier, int page, int rows);

        Task<INotification> Get(string notificationIdentifier);

        Task Acknowledge(string notificationIdentifier);

        Task Dismiss(string notificationIdentifier);
    }
}