using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Borg.Client.ViewComponents
{

    public class MyComponent : ViewComponent
    {
        public  IViewComponentResult Invoke()
        {
            return View("Alternate");
        }
    }
}