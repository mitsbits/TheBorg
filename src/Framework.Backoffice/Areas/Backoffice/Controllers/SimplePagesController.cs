using Borg.Framework.MVC;
using Borg.Framework.System;
using Microsoft.AspNetCore.Authorization;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Authorize]
    public class SimplePagesController : BackofficeController
    {
        public SimplePagesController(ISystemService<BorgSettings> systemService) : base(systemService)
        {
        }
    }
}