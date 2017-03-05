using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Borg.Framework.GateKeeping.Models
{
    public class BorgRole : IdentityRole
    {
        public BorgRole(string roleName):base(roleName) { }
    }
}