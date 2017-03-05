
using Borg.Framework.Services.Notifications;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Borg.Framework.GateKeeping.Models;

namespace Borg.Framework.Backoffice.Areas.Backoffice.ViewComponents
{
    public class MainHeaderNavBarViewComponent : ViewComponent
    {
        private readonly IUserNotificationsStore _userNotifications;

        public MainHeaderNavBarViewComponent(IUserNotificationsStore userNotifications)
        {
            _userNotifications = userNotifications;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claims = User.Identity as ClaimsIdentity;
            var a = claims.FindFirst(x => x.Type == BorgClaims.Profile.Avatar).Value;
            var n = claims.Name;
            var roles = claims.Claims.Where(c => c.Type == claims.RoleClaimType).Select(x => x.Value);
            var user = new SidebarUserInfoViewModel() { Nickname = n, Avatar = a, Roles = roles?.ToArray(), Id = claims.GetSubjectId() };
            var pending = await _userNotifications.Pending(User.GetSubjectId(), 1, 10);
            user.UserNotifications = pending?.ToArray();
            return View(new MainHeaderNavBarViewModel() { UserInfo = user, });
        }
    }

    public class MainHeaderNavBarViewModel
    {
        public SidebarUserInfoViewModel UserInfo { get; set; }
    }
}