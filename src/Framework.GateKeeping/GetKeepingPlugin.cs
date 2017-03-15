using Borg.Framework.System;

namespace Borg.Framework.GateKeeping
{
    public class GateKeepingPlugin : IBorgPlugin
    {
        public GateKeepingPlugin()
        {
            IdentityDescriptor = BuildDescriptor();
            Features = new IBorgFeature[] { new GateKeepingFeature(), new BorgSystemFeature(), };
        }

        public IBorgIdentityDescriptor IdentityDescriptor { get; }
        public IBorgFeature[] Features { get; }

        private static IBorgIdentityDescriptor BuildDescriptor()
        {
            return new IdentityDescriptorBuilder()
                .AddRolesToBorgClaim("GateKeeping", "manage", SystemRoles.SysAdmin, SystemRoles.AppAdmin)
                .AddRolesToBorgClaim("GateKeeping", "create", SystemRoles.Author)
                .AddRolesToBorgClaim("GateKeeping", "view", SystemRoles.ReadOnly)
                .Descriptor();
        }
    }

    public class GateKeepingFeature : BorgFeature
    {
        public GateKeepingFeature() : base("Gate Keeping")
        {
        }
    }
}