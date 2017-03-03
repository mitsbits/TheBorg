using Borg.Framework.System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Borg.Framework.Sql.UserNotifications
{
    public class UserNotificationsDbContextFactory : BorgDbContextFactory<UserNotificationsDbContext, BorgSettings>
    {

        public UserNotificationsDbContextFactory() : base()
        {
            OnMigrationConfiguring += (out string connectionString, out Action<SqlServerDbContextOptionsBuilder> action) =>
                {
                    connectionString =
                        "Server=.\\x2014;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
                    action = builder => builder.MigrationsAssembly("Framework.Backoffice");
                };
        }
    }
}