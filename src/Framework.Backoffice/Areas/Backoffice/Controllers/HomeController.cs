using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;

using Borg.Framework.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    //[SecurityHeaders]
    [Authorize]
    public class HomeController : BackofficeController
    {
        public HomeController(IBackofficeService<BorgSettings> systemService) : base(systemService)
        {
            var descriptor = System.BorgHost.IdentityDescriptor();
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