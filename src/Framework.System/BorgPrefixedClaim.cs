



using System.Security.Claims;

namespace Borg.Framework.System
{
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