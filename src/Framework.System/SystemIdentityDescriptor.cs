using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Borg.Framework.System
{
    internal class SystemIdentityDescriptor : IBorgPluginIdentityDescriptor
    {
        private readonly Lazy<string[]> _roleNames = 
            new Lazy<string[]>(()=> 
            {
                var system = new SystemRoles();
                return system.Select(x => x.Value).ToArray();
            });

        public string[] RoleNames => _roleNames.Value;
        public IEnumerable<Claim> Claims(string role)
        {
            return new Claim[0];
        }
    }

    public class BorgSystemPlugIn : IBorgPlugin
    {
        public BorgSystemPlugIn()
        {
            IdentityDescriptor = new SystemIdentityDescriptor();
        }
        public IBorgPluginIdentityDescriptor IdentityDescriptor { get; }
    }
}