using System.Collections.Generic;
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
        IBorgFeature[] Features { get; }
    }

    public interface IBorgIdentityDescriptor
    {
        string[] RoleNames { get; }

        IEnumerable<Claim> Claims(string role);
    }

    public interface IBorgFeature
    {
        string Name { get; }
        bool Enabled { get; }
    }
}