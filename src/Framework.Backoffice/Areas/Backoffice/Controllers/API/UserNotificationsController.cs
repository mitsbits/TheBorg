using Borg.Framework.Services.Notifications;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers.API
{
    [Area("Backoffice")]
    [Route("[area]/api/[controller]")]
    public class UserNotificationsController : Controller
    {
        private readonly IUserNotificationService _service;

        public UserNotificationsController(IUserNotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var model = await _service.Find(User.GetSubjectId(), 1, 100);
            return new OkObjectResult(model);
        }
    }
}