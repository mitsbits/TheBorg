using System.Security.Claims;
using System.Threading.Tasks;
using Borg.Framework.Backoffice.Identity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.Backoffice.Areas.Backoffice.ViewComponents
{
    public class MainHeaderNavBarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claims = User.Identity as ClaimsIdentity;
            var a = claims.FindFirst(x => x.Type == BorgClaims.Profile.Avatar).Value;
            var n = claims.Name;
            var user = new SidebarUserInfoViewModel() {Nickname = n, Avatar = a};
            return View(new MainHeaderNavBarViewModel() { UserInfo = user}) ;
        }
    }

    public class MainHeaderNavBarViewModel
    {
        public SidebarUserInfoViewModel UserInfo { get; set; }
    }
}