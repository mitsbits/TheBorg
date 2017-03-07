using Borg.Framework.System;

namespace Borg.Framework.Media
{
    public class MediaPlugin : IBorgPlugin
    {
        public MediaPlugin()
        {
            IdentityDescriptor = BuildDescriptor();
        }

        public IBorgIdentityDescriptor IdentityDescriptor { get; }

        private static IBorgIdentityDescriptor BuildDescriptor()
        {
            return new IdentityDescriptorBuilder()
                .AddRolesToBorgClaim("Media", "manage", SystemRoles.SysAdmin, SystemRoles.AppAdmin, SystemRoles.Editor)
                .AddRolesToBorgClaim("Media", "create", SystemRoles.Author)
                .AddRolesToBorgClaim("Media", "view", SystemRoles.ReadOnly)
                .Descriptor();
        }
    }
}