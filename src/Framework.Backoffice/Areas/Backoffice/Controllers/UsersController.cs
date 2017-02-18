using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    public class UsersController :BackofficeController
    {
        public UsersController(ISystemService<BorgSettings> systemService) : base(systemService)
        {
        }

        public IActionResult Index()
        {
            PageContent(new PageContent()
            {
                Title = "Users"
            });
            Logger.LogDebug("{@request}", Request);
            return View();
        }
    }
}
