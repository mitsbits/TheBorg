using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.Services.Notifications;
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
        private readonly INotificationService _notifications;

        public HomeController(ISystemService<BorgSettings> systemService, INotificationService notifications) : base(systemService)
        {
            _notifications = notifications;
        }

        public async Task<IActionResult> Index()
        {
            PageContent(new PageContent()
            {
                Title = System.Settings.Backoffice.Application.Title,
                Subtitle = "Dashbord"
            });
            await _notifications.Info(User.Identity.Name, PageContent<PageContent>().Title,
                PageContent<PageContent>().Subtitle);
            await _notifications.Error(User.Identity.Name, PageContent<PageContent>().Title,
                PageContent<PageContent>().Subtitle);
            await _notifications.Success(User.Identity.Name, PageContent<PageContent>().Title,
                PageContent<PageContent>().Subtitle);
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