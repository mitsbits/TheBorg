using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Borg.Framework.System
{
    internal class SystemIdentityDescriptor : IBorgIdentityDescriptor
    {
        private readonly Lazy<string[]> _roleNames =
            new Lazy<string[]>(() =>
            {
                var system = new SystemRoles();
                return system.Select(x => x.Value).ToArray();
            });

        public string[] RoleNames => _roleNames.Value;

        public IEnumerable<Claim> Claims(string role)
        {
            if (role.Equals(SystemRoles.SysAdmin)) return new[] { new BorgPrefixedClaim("System", "Install"), new BorgPrefixedClaim("System", "Configure") };
            return new Claim[0];
        }
    }
}