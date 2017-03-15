using Borg.Framework.System;

namespace Borg.Framework.UserNotifications
{
    public class UserNotificationsPlugin : IBorgPlugin
    {
        public UserNotificationsPlugin()
        {
            IdentityDescriptor = BuildDescriptor();
        }

        public IBorgIdentityDescriptor IdentityDescriptor { get; }

        private static IBorgIdentityDescriptor BuildDescriptor()
        {
            return new IdentityDescriptorBuilder()
                .AddRolesToBorgClaim("UserNotifications", "manage", SystemRoles.SysAdmin, SystemRoles.AppAdmin, SystemRoles.Editor)
                .Descriptor();
        }

        public IBorgFeature[] Features { get; }
    }
}