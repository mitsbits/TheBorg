using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Borg.Framework.GateKeeping.Models;
using Borg.Framework.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.UserNotifications
{
    [BorgModule]
    public class UserNotificationsViewComponent : ViewComponent
    {
        private readonly IUserNotificationsStore _userNotifications;
        private readonly UserManager<BorgUser> _userManager;

        public UserNotificationsViewComponent(IUserNotificationsStore userNotifications, UserManager<BorgUser> userManager)
        {
            _userNotifications = userNotifications;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string view = "")
        {
            var id = (await _userManager.GetUserAsync(User as ClaimsPrincipal)).Id;

            var user = new UserNotificationsViewModel();
            var pending = await _userNotifications.Pending(id, 1, 10);
            user.UserNotifications = pending?.ToArray();
            return (string.IsNullOrWhiteSpace(view)) ? View(user) : View(view, user);
        }
    }
}