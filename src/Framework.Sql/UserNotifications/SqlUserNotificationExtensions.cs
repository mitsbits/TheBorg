using System;
using Borg.Framework.Services.Notifications;
using Borg.Framework.Sql.UserNotifications;
using Borg.Framework.System;
using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SqlUserNotificationExtensions
    {
        public static void AddSqlUserNotifications<TSettings>(this IServiceCollection services, TSettings settings)
            where TSettings : BorgSettings
        {
            services.AddDbContext<UserNotificationsDbContext>(builder => builder.UseSqlServer(settings.Backoffice.Application.Data.Relational.ConnectionStringIndex["borg"]));
            services.AddSingleton<IDbContextFactory<UserNotificationsDbContext>, UserNotificationsDbContextFactory>();
            services.AddSingleton<IUserNotificationsStore, SqlUserNotificationsStore>();
            services.AddScoped<IRepository, UserNotificationsDbRepository<UserNotification>>();
            services.AddScoped<ICRUDRespoditory<UserNotification>, UserNotificationsDbRepository<UserNotification>>();
        }


    }
}