using System;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.System.Backoffice.UserSession
{
    public class UserSessionViewComponent : ViewComponent
    {
        private readonly IUserSession _userSession;

        public UserSessionViewComponent(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public async Task<IViewComponentResult> InvokeAsync(string view = "")
        {
            var model = new UserSessionViewModel()
            {
                UserIdentifier = _userSession.UserIdentifier,
                SessionStart = _userSession.SessionStart,
                MenuIsCollapsed = _userSession.MenuIsCollapsed()
            };
            return (string.IsNullOrWhiteSpace(view)) ? View(model) : View(view, model);
        }
    }

    public class UserSessionViewModel
    {
        public string UserIdentifier { get; set; }
        public DateTimeOffset SessionStart { get; set; }
        public bool MenuIsCollapsed { get; set; }
        public string RedirectUrl { get; set; }
    }


}
