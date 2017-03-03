using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Borg.Framework.System;

namespace Borg.Client.Controllers
{
    public class KokoController : FrameworkController
    {
        public KokoController(ISystemService<BorgSettings> loggerFactory) : base(loggerFactory)
        {
        }

        public async Task<IActionResult> Index()
        {
            var content = PageContent<PageContent>();
            content.Title = "this came from controller";
            PageContent(content);

           // await Broadcaster.BroadcastMessage(new message() { Message = "hallo" });

            return View();
        }

        private class message
        {
            public string Message { get; set; }
        }
    }
}