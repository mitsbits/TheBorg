using System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;

namespace Borg.Framework.UserNotifications.Sql
{
    public class UserNotificationsDbRepository<T> : BaseReadWriteRepository<T, UserNotificationsDbContext>, ICRUDRespoditory<T> where T : class
    {
        public override UserNotificationsDbContext DbContext { get; }

        public UserNotificationsDbRepository(UserNotificationsDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            DbContext = dbContext;
        }
    }
}