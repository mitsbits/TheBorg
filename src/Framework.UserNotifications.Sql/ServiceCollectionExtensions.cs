using Borg.Framework.System;
using Borg.Framework.UserNotifications;
using Borg.Framework.UserNotifications.Sql;
using Borg.Infra.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBorgUserNotificationsForSql<TSettings>(this IServiceCollection services, TSettings settings)
            where TSettings : BorgSettings
        {
            services.AddSingleton<IBorgPlugin, UserNotificationsSqlPlugin>();
            services.AddDbContext<UserNotificationsDbContext>(builder => builder.UseSqlServer(settings.Backoffice.Application.Data.Relational.ConnectionStringIndex["borg"]));
            services.AddSingleton<IDbContextFactory<UserNotificationsDbContext>, UserNotificationsDbContextFactory>();
            services.AddSingleton<IUserNotificationsStore, SqlUserNotificationsStore>();
            services.AddScoped<IRepository, UserNotificationsDbRepository<UserNotification>>();
            services.AddScoped<ICRUDRespoditory<UserNotification>, UserNotificationsDbRepository<UserNotification>>();
        }
    }
}