using Borg.Framework.GateKeeping.Models;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;

namespace Borg.Framework.GateKeeping.Data.Seeds
{
    public class IdentityDbSeed
    {
        public static void InitialiseIdentity(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var ctx = serviceScope.ServiceProvider.GetRequiredService<GateKeepingDbContext>();
                ctx.Database.Migrate();
                var umanager = serviceScope.ServiceProvider.GetRequiredService<UserManager<BorgUser>>();
                var rmanager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var adminrole =
                     rmanager.Roles.FirstOrDefault(
                        x => x.NormalizedName.Equals("admin", StringComparison.OrdinalIgnoreCase));

                if (adminrole == null)
                {
                    adminrole = new IdentityRole("admin");
                    var r = rmanager.CreateAsync(adminrole).Result;
                    r = rmanager.AddClaimAsync(adminrole, new Claim(JwtClaimTypes.NickName, "adminaras")).Result;
                }

                var borgrole =
                     rmanager.Roles.FirstOrDefault(
                        x => x.NormalizedName.Equals("borg", StringComparison.OrdinalIgnoreCase));

                if (borgrole == null)
                {
                    borgrole = new IdentityRole("borg");
                    var r = rmanager.CreateAsync(borgrole).Result;
                    r = rmanager.AddClaimAsync(borgrole, new Claim(JwtClaimTypes.NickName, "borgaras")).Result;
                }

                var user = umanager.FindByNameAsync("mitsbits").Result;
                if (user == null)
                {
                    user = new BorgUser
                    {
                        Email = "mitsbits@gmail.com",
                        UserName = "mitsbits",
                        LockoutEnabled = false,
                    };

                    var re = umanager.CreateAsync(user, "123456").Result;
                    re = umanager.SetLockoutEnabledAsync(user, false).Result;
                    re = umanager.AddClaimAsync(user, new Claim(BorgSpecificClaims.Profile.Avatar, "https://pbs.twimg.com/profile_images/586144633526300673/Xla8gpkH.jpg")).Result;

                    umanager.AddToRolesAsync(user, new[] { "admin", "borg" });
                }
            }
        }
    }
}