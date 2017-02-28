using Borg.Infra.Relational;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Framework.Services.Notifications
{
    public class InMemoryNotificationService : INotificationService
    {
        private static ConcurrentDictionary<string, List<Notification>> _db = new ConcurrentDictionary<string, List<Notification>>();
        private static ConcurrentDictionary<string, string> _index = new ConcurrentDictionary<string, string>();

        public Task<IPagedResult<INotification>> Find(string recipientIdentifier, int page, int rows)
        {
            if (_db.ContainsKey(recipientIdentifier))
            {
                List<Notification> list;
                if (_db.TryGetValue(recipientIdentifier, out list))
                {
                    var count = list.Count;
                    if (count > (page * rows)) page = (count / rows) + 1;
                    var data =
                        list.OrderByDescending(x => x.Timestamp)
                            .Skip(page - 1)
                            .Take(rows)
                            .Cast<INotification>()
                            .ToArray();
                    var result = new PagedResult<INotification>(data, page, rows, count);
                    return Task.FromResult((IPagedResult<INotification>)result);
                }
            }
            return Task.FromResult(new PagedResult<INotification>(new List<INotification>(), 1, rows, 0) as IPagedResult<INotification>);
        }

        public async Task<INotification> Get(string notificationIdentifier)
        {
            string recipient;
            if (!_index.TryGetValue(notificationIdentifier, out recipient)) return null;
            List<Notification> list;
            if (!_db.TryGetValue(recipient, out list))
            {
                _index.TryRemove(notificationIdentifier, out recipient);
                return null;
            }
            var hit = list.FirstOrDefault(x => x.NotificationIdentifier.Equals(notificationIdentifier));
            return await Task.FromResult(hit);
        }

        public Task Acknowledge(string notificationIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task Dismiss(string notificationIdentifier)
        {
            throw new System.NotImplementedException();
        }

        public Task Add(INotification notification)
        {
            var dto = notification.ToDto();
            List<Notification> list = new List<Notification>();
            _db.GetOrAdd(notification.RecipientIdentifier, list);
            _index.TryAdd(notification.NotificationIdentifier, notification.RecipientIdentifier);
            list.Add(dto);
            _db[dto.RecipientIdentifier] = list;
            return Task.CompletedTask;
        }
    }

    internal static class InMemoryNotificationServiceExtensions
    {
        public static Notification ToDto(this INotification source)
        {
            return new Notification() { Title = source.Title, Message = source.Message, RecipientIdentifier = source.RecipientIdentifier, ResponseStatus = source.ResponseStatus };
        }
    }
}