using Borg.Framework.MVC;
using Borg.Framework.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Borg.Framework.GateKeeping.Models;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Microsoft.AspNetCore.Identity;


namespace Borg.Framework.UserNotifications.Controllers
{
    [Area("Backoffice")]
    [Authorize]
    public class UserNotificationsController : BackofficeController
    {
        private readonly IUserNotificationsStore _service;
        private readonly UserManager<BorgUser> _userManager;

        public UserNotificationsController(IBackofficeService<BorgSettings> systemService, IUserNotificationsStore service, UserManager<BorgUser> userManager) : base(systemService)
        {
            _service = service;
            _userManager = userManager;
        }
 
        public async Task<IActionResult> Index()
        {
            var id = (await _userManager.GetUserAsync(User)).Id;
            var model = await _service.Find(id, Pager.Current, Pager.RowCount);
            PageContent(new PageContent()
            {
                Title = $"Notifications",
                Subtitle = $"Page {model.Page} of {model.TotalPages}"
            });

            return  View(model);
        }

        public async Task<IActionResult> DeleteNotification(string id, string redirect)
        {
            await _service.Dismiss(id);
            return Redirect(redirect);
        }


        [HttpGet("[area]/api/[controller]")]
        public async Task<IActionResult> Get(int p = 0, int r = 0)
        {
            r = (r == 0) ? Backoffice.Settings.Backoffice.Pager.DefaultRowCount : r;
            var id = (await _userManager.GetUserAsync(User)).Id;
            var model = await _service.Find(id, p, r);
            return new OkObjectResult(model);
        }
    }
}