using System;
using Borg.Framework.Sql;
using Borg.Framework.System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Borg.Framework.UserNotifications.Sql
{
    public class UserNotificationsDbContextFactory : BorgDbContextFactory<UserNotificationsDbContext, BorgSettings>
    {
        //TODO: wtf?
        public UserNotificationsDbContextFactory() : base()
        {
            OnMigrationConfiguring += (out string connectionString, out Action<SqlServerDbContextOptionsBuilder> action) =>
            {
                connectionString =
                    "Server=.\\SQL2016;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
                        //"Server=.\\x2014;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
                    action = builder => builder.MigrationsAssembly("Framework.Backoffice");
                };
        }
    }
}