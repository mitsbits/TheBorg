using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    //[SecurityHeaders]
    [Authorize]
    public class HomeController : BackofficeController
    {
        private readonly BorgSettings _settings;
        public HomeController(ISystemService<BorgSettings> systemService, BorgSettings settings) : base(systemService)
        {
            _settings = settings;
        }

        public IActionResult Index()
        {
             PageContent(new PageContent()
             {
                 Title = _settings.Backoffice.Application.Title,
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