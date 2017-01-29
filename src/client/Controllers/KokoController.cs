using Borg.Framework.MVC.BuildingBlocks.Devices;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Client.Controllers
{
    public class KokoController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ContentInfo = new PageContent() { Title = "this came from controller" };
            return View();
        }
    }
}