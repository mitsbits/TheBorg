﻿using System.Threading.Tasks;
using Framework.System;
using Microsoft.AspNetCore.Mvc;

namespace Borg.Famework.Backoffice.Areas.Backoffice
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
