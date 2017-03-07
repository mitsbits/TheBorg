using Borg.Framework.System;

namespace Borg.Framework.UserNotifications.Sql
{
    public class UserNotificationsSqlPlugin : IBorgPlugin
    {
        public UserNotificationsSqlPlugin()
        {
            IdentityDescriptor = new IdentityDescriptorBuilder().Descriptor();
        }

        public IBorgIdentityDescriptor IdentityDescriptor { get; }
    }
}