using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Borg.Framework.System
{
    public interface IBorgHost
    {
        IBorgPluginIdentityDescriptor[] IdentityDescriptors { get; }
        IBorgPlugin[] RegisteredPlugins { get; }
    }

    public interface IBorgPlugin
    {
        IBorgPluginIdentityDescriptor IdentityDescriptor { get; }
    }

    public interface IBorgPluginIdentityDescriptor
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
        public BorgHost(IBorgPlugin[] registeredPlugins)
        {
            RegisteredPlugins = registeredPlugins;
         
        }
        public IBorgPlugin[] RegisteredPlugins { get; }
        public IBorgPluginIdentityDescriptor[] IdentityDescriptors => RegisteredPlugins.Select(x => x.IdentityDescriptor).ToArray();
    }
}