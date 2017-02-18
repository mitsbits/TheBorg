﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Borg.Framework.Backoffice.Identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class BorgUser : IdentityUser
    {

    }

    public class BorgClaims
    {
        public class Profile
        {
            public const string Avatar = "Profile.Avatar";
        }
    }
}