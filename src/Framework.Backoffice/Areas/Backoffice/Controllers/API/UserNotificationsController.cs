using Borg.Framework.Services.Notifications;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Borg.Framework.MVC;
using Borg.Framework.System;
using Microsoft.AspNetCore.Authorization;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers.API
{
    [Area("Backoffice")]
    [Route("[area]/api/[controller]")][Authorize]
    public class UserNotificationsController : BackofficeController
    {
        private readonly IUserNotificationsStore _service;



        public UserNotificationsController(IBackofficeService<BorgSettings> systemService, IUserNotificationsStore service) :base(systemService)
        {
            _service = service;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get(int p = 0, int r = 0)
        {
            r = (r == 0) ? Backoffice.Settings.Backoffice.Pager.DefaultRowCount : r;
            var model = await _service.Find(User.GetSubjectId(), p, r);
            return new OkObjectResult(model);
        }
    }
}