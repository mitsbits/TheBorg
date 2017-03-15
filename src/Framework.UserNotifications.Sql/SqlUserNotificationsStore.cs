using System.Threading.Tasks;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using Borg.Infra.Relational;

namespace Borg.Framework.UserNotifications.Sql
{
    [BorgModule]
    public class SqlUserNotificationsStore : IUserNotificationsStore
    {
        private readonly IEventBus _events;
        private readonly UserNotificationsDbContext _db;
        private readonly ICRUDRespoditory<UserNotification> _repo;

        public SqlUserNotificationsStore(IEventBus events, UserNotificationsDbContext db)
        {
            _events = events;
            _db = db;
            _repo = new UserNotificationsDbRepository<UserNotification>(_db);
        }

        public async Task Add(string recipientIdentifier, ResponseStatus responseStatus, string title, string message)
        {
            var notigication = await _repo.CreateAsync(new UserNotification()
            {
                Title = title,
                Message = message,
                RecipientIdentifier = recipientIdentifier,
                ResponseStatus = responseStatus
            });
            await _db.SaveChangesAsync();
            _events.Publish(new NotificationCreatedEvent(notigication));
        }

        public async Task<IPagedResult<IUserNotification>> Find(string recipientIdentifier, int page, int rows)
        {
            return await _repo.FindAsync(x => x.RecipientIdentifier.Equals(recipientIdentifier), page, rows,
                new[] { new OrderByInfo<UserNotification>() { Ascending = false, Property = x => x.Timestamp } });
        }

        public async Task<IPagedResult<IUserNotification>> Pending(string recipientIdentifier, int page, int rows)
        {
            return await _repo.FindAsync(x => x.RecipientIdentifier.Equals(recipientIdentifier) && !x.Acknowledged, page, rows,
                new[] { new OrderByInfo<UserNotification>() { Ascending = false, Property = x => x.Timestamp } });
        }

        public async Task<IUserNotification> Get(string notificationIdentifier)
        {
            return await _repo.GetAsync(x => x.NotificationIdentifier.Equals(notificationIdentifier));
        }

        public async Task Acknowledge(string notificationIdentifier)
        {
            var hit = await _repo.GetAsync(x => x.NotificationIdentifier.Equals(notificationIdentifier));
            if (hit != null)
            {
                hit.Acknowledged = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task Dismiss(string notificationIdentifier)
        {
            await _repo.DeleteAsync(x => x.NotificationIdentifier.Equals(notificationIdentifier));
            await _db.SaveChangesAsync();
        }
    }
}