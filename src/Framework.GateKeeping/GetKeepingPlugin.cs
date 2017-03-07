using Borg.Framework.System;

namespace Borg.Framework.GateKeeping
{
    public class GateKeepingPlugin : IBorgPlugin
    {
        public GateKeepingPlugin()
        {
            IdentityDescriptor = BuildDescriptor();
        }

        public IBorgIdentityDescriptor IdentityDescriptor { get; }

        private static IBorgIdentityDescriptor BuildDescriptor()
        {
            return new IdentityDescriptorBuilder()
                .AddRolesToBorgClaim("GateKeeping", "manage", SystemRoles.SysAdmin, SystemRoles.AppAdmin)
                .AddRolesToBorgClaim("GateKeeping", "create", SystemRoles.Author)
                .AddRolesToBorgClaim("GateKeeping", "view", SystemRoles.ReadOnly)
                .Descriptor();
        }
    }
}