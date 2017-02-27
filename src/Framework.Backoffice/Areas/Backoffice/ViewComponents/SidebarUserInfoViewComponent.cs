using System.Security.Claims;
using System.Threading.Tasks;
using Borg.Framework.Backoffice.Identity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.Backoffice.Areas.Backoffice.ViewComponents
{
    public class SidebarUserInfoViewComponent : ViewComponent
    {
       
        public SidebarUserInfoViewComponent()
        {
         
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claims = User.Identity as ClaimsIdentity;
            var a = claims.FindFirst(x => x.Type == BorgClaims.Profile.Avatar).Value;
            var n = claims.Name;
            return  View(new SidebarUserInfoViewModel() { Avatar =  a, Nickname = n}) ;
   
        }
    }

    public class SidebarUserInfoViewModel
    {
        public string Avatar { get; set; }
        public string Nickname { get; set; }

        public string[] Roles { get; set; } = new string[0];
    }
}