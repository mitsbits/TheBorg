using System.Security.Claims;
using System.Threading.Tasks;
using Borg.Framework.System;
using Borg.Framework.Backoffice.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Framework.Backoffice.Areas.Backoffice.ViewComponents
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
        private readonly UserManager<BorgUser> _manager;
        public SidebarUserInfoViewComponent(UserManager<BorgUser> manager)
        {
            _manager = manager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claims = User.Identity as ClaimsIdentity;
            var a = claims.FindFirst(x => x.Type == BorgClaims.Profile.Avatar).Value;
            var n = claims.Name;
            return  View(new SidebarUserInfoViewModel() { Avatar =  a, Nickname = n}) ;
   
        }
    }

    public class SidebarUserInfoViewModel
    {
        public string Avatar { get; set; }
        public string Nickname { get; set; }
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
        private readonly BorgSettings _borg; 
        public MainHeaderLogoViewComponent(BorgSettings borg)
        {
            _borg = borg;
        }
        public Task<IViewComponentResult> InvokeAsync()
        {
            var result = View(new MainHeaderLogoViewModel(_borg.Backoffice.Application)) as IViewComponentResult;
            return Task.FromResult(result);
        }
    }


    public class MainHeaderLogoViewModel
    {
        public MainHeaderLogoViewModel(ApplicationSettings settings)
        {
            Title= settings.Title;
            Logo = settings.Logo;

        }
        public string Logo { get; }
        public string Title { get; }
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
