using System.Collections.Generic;
using System.Security.Claims;

namespace Borg.Framework.System
{
    public class IdentityDescriptorBuilder
    {
        private readonly Dictionary<string, List<Claim>> _data;

        public IdentityDescriptorBuilder()
        {
            _data = new Dictionary<string, List<Claim>>();
        }


        public virtual IdentityDescriptorBuilder AddClaim(string role, string type, string value)
        {
            var list = _data.ContainsKey(role) ? _data[role] : new List<Claim>();
            list.Add(new Claim(type, value));
            _data[role] = list;
            return this;
        }
        public virtual IdentityDescriptorBuilder AddBorgClaim(string role, string type, string value)
        {
            var list = _data.ContainsKey(role) ? _data[role] : new List<Claim>();
            list.Add(new BorgPrefixedClaim(type, value));
            _data[role] = list;
            return this;
        }
        public virtual IdentityDescriptorBuilder AddRole(string role)
        {
            if (!_data.ContainsKey(role)) _data.Add(role, new List<Claim>());
            return this;
        }
        public virtual IdentityDescriptorBuilder AddRoles(params string[] roles)
        {
            foreach (var role in roles)
            {
                if (!_data.ContainsKey(role)) _data.Add(role, new List<Claim>());
            }
            return this;
        }
        public virtual IdentityDescriptorBuilder AddRolesToClaim(string type, string value,params string[] roles)
        {
            foreach (var role in roles)
            {
                AddClaim(role, type, value);
            }
            return this;
        }
        public virtual IdentityDescriptorBuilder AddRolesToBorgClaim(string type, string value, params string[] roles)
        {
            foreach (var role in roles)
            {
                AddBorgClaim(role, type, value);
            }
            return this;
        }

        public IBorgIdentityDescriptor Descriptor()
        {
            return new IdentityDescriptorDto(_data);
        }
    }
}