using System.Threading.Tasks;
using Borg.Framework.MVC;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.System.Backoffice.UserSession
{
    public class UserSessionController : BackofficeController
    {
        private readonly IUserSession _userSession;
        public UserSessionController(IBackofficeService<BorgSettings> systemService, IUserSession userSession) : base(systemService)
        {
            _userSession = userSession;
        }

        public IActionResult SessionSettings(UserSessionViewModel model)
        {
            _userSession.MenuIsCollapsed(model.MenuIsCollapsed);
            return Redirect(model.RedirectUrl);
        }
    }
}