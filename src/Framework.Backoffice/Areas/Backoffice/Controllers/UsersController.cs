﻿using Borg.Framework.Backoffice.Pages.Commands;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Areas.Backoffice.Controllers
{
    [Area("backoffice")]
    public class UsersController : BackofficeController
    {
        private readonly ICommandBus _bus;

        public UsersController(ISystemService<BorgSettings> systemService, ICommandBus bus) : base(systemService)
        {
            _bus = bus;
        }

        public async Task<IActionResult> Index()
        {
            PageContent(new PageContent()
            {
                Title = "Users"
            });

            await _bus.Process(new SimplePageCreateCommand(User.Identity) { Title = "test", Path = "test" });
            Logger.LogCritical("{@request}", Request);
            return View();
        }
    }
}