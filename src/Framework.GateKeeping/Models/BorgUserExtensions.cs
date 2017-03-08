using System.Linq;
using Borg.Framework.GateKeeping.Models;

namespace Borg
{
    public static class BorgUserExtensions
    {
        public static string AvatatUrl(this BorgUser user)
        {
            return user.Claims.Any(x => x.ClaimType.Equals(BorgSpecificClaims.Profile.Avatar))
                ? user.Claims.First(x => x.ClaimType.Equals(BorgSpecificClaims.Profile.Avatar)).ClaimValue
                : string.Empty;
        }
    }
}