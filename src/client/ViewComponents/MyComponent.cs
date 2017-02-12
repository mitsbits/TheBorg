using Microsoft.AspNetCore.Mvc;

namespace Borg.Client.ViewComponents
{
    public class MyComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Alternate");
        }
    }
}