namespace Borg.Framework.System
{
    public class BorgSystemPlugIn : IBorgPlugin
    {
        public BorgSystemPlugIn()
        {
            IdentityDescriptor = new SystemIdentityDescriptor();
            Features = new IBorgFeature[] { new BorgSystemFeature(), };
        }

        public IBorgIdentityDescriptor IdentityDescriptor { get; }
        public IBorgFeature[] Features { get; }
    }

    public class BorgSystemFeature : BorgFeature
    {
        public BorgSystemFeature() : base("System")
        {
        }
    }
}