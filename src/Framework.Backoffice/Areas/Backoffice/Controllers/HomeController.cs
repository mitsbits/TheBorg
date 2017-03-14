using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;

using Borg.Framework.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Borg.Framework.System.Backoffice.UserSession;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    //[SecurityHeaders]
    [Authorize]
    public class HomeController : BackofficeController
    {
        private readonly IUserSession _userSession;

        public HomeController(IBackofficeService<BorgSettings> systemService, IUserSession userSession) : base(systemService)
        {
            _userSession = userSession;

            var lkj = _userSession.SessionStart;
            _userSession.Setting("myKey", "myValue");

            var val = _userSession.Setting<string>("myKey");
        }

        public async Task<IActionResult> Index()
        {
            PageContent(new PageContent()
            {
                Title = System.Settings.Backoffice.Application.Title,
                Subtitle = "Dashbord"
            });

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        [Route("/Error")]
        public IActionResult Error()
        {
            PageContent(new PageContent()
            {
                Title = "ApplicationError",
                Subtitle = "Dashbord"
            });
            return View();
        }
    }
}