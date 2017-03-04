using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Framework.Services.Notifications
{
    public class InMemoryUserNotificationsStore : IUserNotificationsStore
    {
        private readonly IEventBus _events;
        private static ConcurrentDictionary<string, List<UserNotification>> _db = new ConcurrentDictionary<string, List<UserNotification>>();
        private static ConcurrentDictionary<string, string> _index = new ConcurrentDictionary<string, string>();

        public InMemoryUserNotificationsStore(IEventBus events)
        {
            _events = events;
        }

        public Task<IPagedResult<IUserNotification>> Find(string recipientIdentifier, int page, int rows)
        {
            if (_db.ContainsKey(recipientIdentifier))
            {
                List<UserNotification> list;
                if (_db.TryGetValue(recipientIdentifier, out list))
                {
                    var count = list.Count;
                    var totalPages = (int)Math.Ceiling((double)count / rows);
                    if (page > totalPages) page = totalPages;

                    var data =
                        list.OrderByDescending(x => x.Timestamp)
                            .Skip(page - 1)
                            .Take(rows)
                            .Cast<IUserNotification>()
                            .ToArray();
                    var result = new PagedResult<IUserNotification>(data, page, rows, count);

                    return Task.FromResult((IPagedResult<IUserNotification>)result);
                }
            }
            return Task.FromResult(new PagedResult<IUserNotification>(new List<IUserNotification>(), 1, rows, 0) as IPagedResult<IUserNotification>);
        }

        public async Task<IUserNotification> Get(string notificationIdentifier)
        {
            string recipient;
            if (!_index.TryGetValue(notificationIdentifier, out recipient)) return null;
            List<UserNotification> list;
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
            List<UserNotification> list;
            if (!_db.TryGetValue(recipient, out list))
            {
                var hit = list.Single(x => x.NotificationIdentifier.Equals(notificationIdentifier, StringComparison.OrdinalIgnoreCase));
                hit.Acknowledged = true;
                return Task.CompletedTask;
            }
            return null;
        }

        public Task Dismiss(string notificationIdentifier)
        {
            string recipient;
            if (!_index.TryGetValue(notificationIdentifier, out recipient)) return null;
            List<UserNotification> list;
            if (!_db.TryGetValue(recipient, out list))
            {
                var hit = list.Single(x => x.NotificationIdentifier.Equals(notificationIdentifier, StringComparison.OrdinalIgnoreCase));
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

        public Task<IPagedResult<IUserNotification>> Pending(string recipientIdentifier, int page, int rows)
        {
            if (_db.ContainsKey(recipientIdentifier))
            {
                List<UserNotification> list;
                if (_db.TryGetValue(recipientIdentifier, out list))
                {
                    var count = list.Count(x => !x.Acknowledged);
                    if (count > (page * rows)) page = (count / rows) + 1;
                    var data =
                        list.Where(x => !x.Acknowledged)
                            .OrderByDescending(x => x.Timestamp)
                            .Skip(page - 1)
                            .Take(rows)
                            .Cast<IUserNotification>()
                            .ToArray();
                    var result = new PagedResult<IUserNotification>(data, page, rows, count);
                    return Task.FromResult((IPagedResult<IUserNotification>)result);
                }
            }
            return Task.FromResult(new PagedResult<IUserNotification>(new List<IUserNotification>(), 1, rows, 0) as IPagedResult<IUserNotification>);
        }

        public Task Add(string recipientIdentifier, ResponseStatus responseStatus, string title, string message)
        {
            var dto = new UserNotification() { Title = title, Message = message, RecipientIdentifier = recipientIdentifier, ResponseStatus = responseStatus };
            List<UserNotification> list;
            if (!_db.TryGetValue(dto.RecipientIdentifier, out list))
            {
                list = new List<UserNotification>();
                _db.TryAdd(dto.RecipientIdentifier, list);
            }
            _index.TryAdd(dto.NotificationIdentifier, dto.RecipientIdentifier);
            list.Add(dto);
            var task = _events.Publish(new NotificationCreatedEvent(dto));
            return task;
        }
    }

    internal static class InMemoryNotificationServiceExtensions
    {
        public static UserNotification ToDto(this IUserNotification source)
        {
            return new UserNotification() { Title = source.Title, Message = source.Message, RecipientIdentifier = source.RecipientIdentifier, ResponseStatus = source.ResponseStatus };
        }
    }
}