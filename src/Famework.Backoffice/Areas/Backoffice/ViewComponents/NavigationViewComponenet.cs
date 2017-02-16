using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Famework.Backoffice.Areas.Backoffice.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }

    public class SidebarMenuViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }


    public class SidebarUserInfoViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }
    public class SidebarSearchFormViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }


    public class MainHeaderViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }


    public class MainHeaderLogoViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }

    public class MainHeaderNavBarViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }

    public class MainFooterViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }

    public class HiddenSidebarViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View() as IViewComponentResult;
            return Task.FromResult(result);
        }
    }

}
