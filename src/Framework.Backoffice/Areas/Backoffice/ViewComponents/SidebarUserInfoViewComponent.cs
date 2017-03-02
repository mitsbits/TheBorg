using Borg.Framework.Backoffice.Identity.Models;
using Borg.Framework.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Areas.Backoffice.ViewComponents
{
    public class SidebarUserInfoViewComponent : ViewComponent
    {
        //private readonly INotificationService _notifications;
        public SidebarUserInfoViewComponent(/*INotificationService notifications*/)
        {
            //_notifications = notifications;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claims = User.Identity as ClaimsIdentity;
            var a = claims.FindFirst(x => x.Type == BorgClaims.Profile.Avatar).Value;
            var n = claims.Name;
            var model = new SidebarUserInfoViewModel() { Avatar = a, Nickname = n };
            // var pending = await _notifications.Pending(User.Identity.Name, 1, 10);
            // model.Notifications = pending?.ToArray();
            return View(model);
        }
    }

    public class SidebarUserInfoViewModel
    {
        public string Id { get; set; }
        public string Avatar { get; set; }
        public string Nickname { get; set; }
        public string[] Roles { get; set; } = new string[0];
        public IUserNotification[] UserNotifications { get; set; } = new IUserNotification[0];
    }
}