namespace Borg.Framework.System
{
    public class BorgSystemPlugIn : IBorgPlugin
    {
        public BorgSystemPlugIn()
        {
            IdentityDescriptor = new SystemIdentityDescriptor();
        }

        public IBorgIdentityDescriptor IdentityDescriptor { get; }
    }
}