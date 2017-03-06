using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Borg.Framework.System
{
    internal class SystemIdentityDescriptor : IBorgIdentityDescriptor
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
            if (role.Equals(SystemRoles.SysAdmin)) return new[] {new BorgPrefixedClaim("System", "Install"), new BorgPrefixedClaim("System", "Configure") };
            return new Claim[0];
        }
    }

    public class BorgSystemPlugIn : BorgPlugin
    {
        public BorgSystemPlugIn()
        {
            IdentityDescriptor = new SystemIdentityDescriptor();
        }
        public override IBorgIdentityDescriptor IdentityDescriptor { get; }
    }

    public abstract class BorgPlugin : IBorgPlugin
    {
        private readonly Dictionary<string, List<BorgPrefixedClaim>> _db = new Dictionary<string, List<BorgPrefixedClaim>>();
        public virtual  IBorgIdentityDescriptor IdentityDescriptor => new IdentityDescriptorDto(_db.ToDictionary(x=>x.Key, x=>x.Value.Cast<Claim>().ToList()));

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

    internal class BorgPrefixedClaim : Claim
    {
        public BorgPrefixedClaim(string type, string value) : base(BorgPrefix(type), value)
        {

        }

        private static string BorgPrefix(string type)
        {
            return $"{SystemRoles.BorgClaimTypePrexix}{type}";
        }
    }
}