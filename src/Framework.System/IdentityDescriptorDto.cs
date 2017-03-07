using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Borg.Framework.System
{
    internal class IdentityDescriptorDto : IBorgIdentityDescriptor
    {
        private readonly Dictionary<string, List<Claim>> _data;

        internal IdentityDescriptorDto(Dictionary<string, List<Claim>> data)
        {
            _data = data;
        }

        internal IdentityDescriptorDto()
        {
            _data = new Dictionary<string, List<Claim>>();
        }

        string[] IBorgIdentityDescriptor.RoleNames => _data.Keys.ToArray();

        IEnumerable<Claim> IBorgIdentityDescriptor.Claims(string role)
        {
            if (!_data.ContainsKey(role)) return new Claim[0];
            return _data[role];
        }
    }
}