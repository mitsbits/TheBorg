using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;

namespace Borg.Framework.System
{
    public class SystemRoles : IEnumerable<Claim>
    {
        public const string BorgClaimTypePrexix = "X-BORG:";
        public const string RoleClaimType       = ClaimTypes.Role;
        public const string SysAdmin            = "SysAdmin";
        public const string AppAdmin            = "AppAdmin";
        public const string Manager             = "Manager";
        public const string Editor              = "Editor";
        public const string Author              = "Author";
        public const string Backoffice          = "Backoffice";
        public const string Guest               = "Guest";
        public const string ReadOnly            = "ReadOnly";

        private static readonly IEnumerable<Claim> _roles;

        static SystemRoles()
        {
            _roles = new[]
            {
                new Claim(RoleClaimType, SysAdmin  ),
                new Claim(RoleClaimType, AppAdmin  ),
                new Claim(RoleClaimType, Manager   ),
                new Claim(RoleClaimType, Editor    ),
                new Claim(RoleClaimType, Author    ),
                new Claim(RoleClaimType, Backoffice),
                new Claim(RoleClaimType, Guest     ),
                new Claim(RoleClaimType, ReadOnly  )
            };
        }

        public IEnumerator<Claim> GetEnumerator()
        {
            return _roles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


}