using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Borg.Framework.System
{
    public interface IBorgHost
    {
        IBorgPlugin[] RegisteredPlugins { get; }
    }

    public interface IBorgPlugin
    {
        IBorgIdentityDescriptor IdentityDescriptor { get; }
    }

    public interface IBorgIdentityDescriptor
    {
        string[] RoleNames { get; }

        IEnumerable<Claim> Claims(string role);
    }

    public interface IBorgFeature
    {
        string Name { get; }
    }

    public class BorgHost : IBorgHost
    {
        public BorgHost(IEnumerable<IBorgPlugin> registeredPlugins)
        {
            RegisteredPlugins = registeredPlugins?.ToArray();
        }

        public IBorgPlugin[] RegisteredPlugins { get; }
    }
}