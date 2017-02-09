using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Borg.Framework.MVC;
using Borg.Infra.CQRS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace Borg.Client.Areas.Backoffice.Controllers
{
    [Authorize]
    [Area("Backoffice")]
    public class HomeController : BackofficeController
    {
        public HomeController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public async Task< IActionResult> Index( )
        {
            //var r = await Commands.Process(new ExpCommand());
            Logger.LogDebug("user is {@user}", User.Identity);

            Logger.LogInformation("user is {@user}", User.Identity);
            return View();
        }
    }



    public class ExpCommand : ICommand
    {
        public ExpCommand()
        {
            var r = new Random();
            ID = r.Next(1, 1000000);
            r = null;
        }
        public int ID { get; }
    }


    public class ExpCommandHandler : IHandlesCommand<ExpCommand>
    {
        public Task<ICommandResult> Execute(ExpCommand message)
        {
            var e = message.ID;
            return Task.FromResult(CommandResult.Create(true));
        }
    }
}
