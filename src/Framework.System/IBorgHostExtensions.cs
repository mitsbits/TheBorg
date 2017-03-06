using Borg.Framework.System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Borg
{
    public static class IBorgHostExtensions
    {
        public static IBorgIdentityDescriptor IdentityDescriptor(this IBorgHost host)
        {
            var data = new Dictionary<string, List<Claim>>();
            var keys =
                host.RegisteredPlugins.SelectMany(x => x.IdentityDescriptor.RoleNames)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
            foreach (var role in keys)
            {
                foreach (var claims in host.RegisteredPlugins.Select(x => x.IdentityDescriptor.Claims(role)))
                {
                    if (data.ContainsKey(role))
                    {
                        var source = data[role];
                        data[role] = claims.Where(c => !source.Contains(c)).Union(source).ToList();
                    }
                    else
                    {
                        data.Add(role, claims.ToList());
                    }
                }
            }
            return new IdentityDescriptorDto(data);
        }
    }
}

