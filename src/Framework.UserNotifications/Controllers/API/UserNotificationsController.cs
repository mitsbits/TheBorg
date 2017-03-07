using Borg.Framework.MVC;
using Borg.Framework.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Borg.Framework.GateKeeping.Models;
using Microsoft.AspNetCore.Identity;

namespace Borg.Framework.UserNotifications.Controllers.API
{
    [Area("Backoffice")]
    [Route("[area]/api/[controller]")]
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

        [HttpGet("")]
        public async Task<IActionResult> Get(int p = 0, int r = 0)
        {
            r = (r == 0) ? Backoffice.Settings.Backoffice.Pager.DefaultRowCount : r;
            var id = (await _userManager.GetUserAsync(User)).Id;
            var model = await _service.Find(id, p, r);
            return new OkObjectResult(model);
        }
    }
}