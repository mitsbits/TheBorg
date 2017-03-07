using Borg.Framework.System;
using Microsoft.Extensions.DependencyInjection;

namespace Borg.Framework.UserNotifications
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBorgUserNotifications<TSettings>(this IServiceCollection services, TSettings settings) where TSettings : BorgSettings
        {
            services.AddSingleton<IBorgPlugin, UserNotificationsPlugin>();
            services.AddSingleton<IUserNotificationsStore, InMemoryUserNotificationsStore>();
        }
    }
}