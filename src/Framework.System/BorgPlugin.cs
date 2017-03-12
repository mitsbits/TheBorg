using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace Borg.Framework.System
{
    [Obsolete("not very handy")]
    public abstract class BorgPlugin : IBorgPlugin
    {
        private readonly Dictionary<string, List<BorgPrefixedClaim>> _db = new Dictionary<string, List<BorgPrefixedClaim>>();
        public virtual IBorgIdentityDescriptor IdentityDescriptor => new IdentityDescriptorDto(_db.ToDictionary(x => x.Key, x => x.Value.Cast<Claim>().ToList()));

        protected virtual void AddClaim(string role, string type, string value)
        {
            var list = _db.ContainsKey(role) ? _db[role] : new List<BorgPrefixedClaim>();
            list.Add(new BorgPrefixedClaim(type, value));
            _db[role] = list;
        }

        protected virtual void AddRole(string role)
        {
            if (!_db.ContainsKey(role)) _db.Add(role, new List<BorgPrefixedClaim>());
        }
    }
}