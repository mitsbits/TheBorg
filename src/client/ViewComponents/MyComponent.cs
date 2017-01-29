using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Borg.Client.ViewComponents
{
    [ViewComponent(Name = "MyComponent")]
    public class MyComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}