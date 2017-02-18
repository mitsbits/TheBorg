using System.Security.Claims;
using Borg.Framework.Backoffice.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Borg.Framework.Backoffice.Identity.Data.Seeds
{
    public class IdentityDbSeed
    {
        public static void InitialiseIdentity(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var umanager = serviceScope.ServiceProvider.GetRequiredService<UserManager<BorgUser>>();
                //var rmanager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //var adminrole =
                //    await rmanager.Roles.FirstOrDefaultAsync(
                //        x => x.NormalizedName.Equals("admin", StringComparison.InvariantCultureIgnoreCase));

                //if (adminrole == null)
                //{
                //    adminrole = new IdentityRole("admin");
                //    await rmanager.CreateAsync(adminrole);
                //    await rmanager.AddClaimAsync(adminrole, new Claim(JwtClaimTypes.NickName, "adminaras"));
                //}

                var user = umanager.FindByNameAsync("mitsbits").Result;
                if (user == null)
                {
                    user = new BorgUser
                    {
                        Email = "mitsbits@gmail.com",
                        UserName = "mitsbits",
                        LockoutEnabled = false,
                    };

                    var re = umanager.CreateAsync(user, "qazwsx123!@#").Result;
                    re = umanager.SetLockoutEnabledAsync(user, false).Result;
                    re = umanager.AddClaimAsync(user, new Claim(BorgClaims.Profile.Avatar, "mitsbits")).Result;

                }


            }
        }
    }
}
