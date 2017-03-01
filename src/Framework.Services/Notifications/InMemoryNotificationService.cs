using Borg.Infra.Relational;
using System;
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
            string recipient;
            if (!_index.TryGetValue(notificationIdentifier, out recipient)) return null;
            List<Notification> list;
            if (!_db.TryGetValue(recipient, out list))
            {
                var hit = list.Single(x => x.NotificationIdentifier.Equals(notificationIdentifier, StringComparison.InvariantCultureIgnoreCase));
                hit.Acknowledged = true;
                return Task.CompletedTask;
            }
            return null;
        }

        public Task Dismiss(string notificationIdentifier)
        {
            string recipient;
            if (!_index.TryGetValue(notificationIdentifier, out recipient)) return null;
            List<Notification> list;
            if (!_db.TryGetValue(recipient, out list))
            {
                var hit = list.Single(x => x.NotificationIdentifier.Equals(notificationIdentifier, StringComparison.InvariantCultureIgnoreCase));
                list.Remove(hit);
                _index.TryRemove(notificationIdentifier, out recipient);
                if (!list.Any())
                {
                    _db.TryRemove(recipient, out list);
                }
                else
                {
                    _db[recipient] = list;
                }
                return Task.CompletedTask;
            }
            return null;
        }

        public Task Add(INotification notification)
        {
            var dto = notification.ToDto();
            List<Notification> list;
            if (!_db.TryGetValue(notification.RecipientIdentifier, out list))
            {
                list = new List<Notification>();
                _db.TryAdd(notification.RecipientIdentifier, list);
            }
            _index.TryAdd(notification.NotificationIdentifier, notification.RecipientIdentifier);
            list.Add(dto);
            //_db[dto.RecipientIdentifier] = list;
            return Task.CompletedTask;
        }

        public Task<IPagedResult<INotification>> Pending(string recipientIdentifier, int page, int rows)
        {
            if (_db.ContainsKey(recipientIdentifier))
            {
                List<Notification> list;
                if (_db.TryGetValue(recipientIdentifier, out list))
                {
                    var count = list.Count(x => !x.Acknowledged);
                    if (count > (page * rows)) page = (count / rows) + 1;
                    var data =
                        list.Where(x => !x.Acknowledged)
                            .OrderByDescending(x => x.Timestamp)
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
    }

    internal static class InMemoryNotificationServiceExtensions
    {
        public static Notification ToDto(this INotification source)
        {
            return new Notification() { Title = source.Title, Message = source.Message, RecipientIdentifier = source.RecipientIdentifier, ResponseStatus = source.ResponseStatus };
        }
    }
}