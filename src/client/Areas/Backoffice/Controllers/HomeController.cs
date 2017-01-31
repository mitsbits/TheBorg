using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Borg.Framework.MVC;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Client.Areas.Backoffice.Controllers
{
    [Area("Backoffice")]
    public class HomeController : BackofficeController
    {
        public IActionResult Index()
        {
            var s = Events.GetType();
            return View();
        }
    }
}
