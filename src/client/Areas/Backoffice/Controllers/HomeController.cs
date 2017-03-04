using Borg.Framework.MVC;
using Borg.Infra.CQRS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Borg.Framework.System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Framework.System.Domain;
using Hangfire;

namespace Borg.Client.Areas.Backoffice.Controllers
{
    [Authorize]
    [Area("Backoffice")]
    public class HomeController : BackofficeController
    {
        private readonly IDbContextScopeFactory _uow;
        private readonly ICRUDRespoditory<Page> _repo;
        private readonly IBackgroundJobClient _jobClient;
        public HomeController(IBackofficeService<BorgSettings> systemService, IDbContextScopeFactory uow, ICRUDRespoditory<Page> repo, IBackgroundJobClient jobClient) : base(systemService)
        {
            _uow = uow;
            _repo = repo;
            _jobClient = jobClient;
        }

        public async Task<IActionResult> Index()
        {
            //var r = await Commands.Process(new ExpCommand());
            Logger.LogDebug("user is {@user}", User.Identity);

            Logger.LogInformation("user is {@user}", User.Identity);

         var ss =   Hangfire.BackgroundJob.Schedule(() => PutToQueue(), TimeSpan.FromMinutes(5));


            using (var db = _uow.Create())
            {
                var c = db.DbContexts;
                var page = _repo.Get(x => x.CQRSKey == string.Empty);
            }
            return View();
        }
        [Queue("borg_priority")]
        public void PutToQueue()
        {
            Debug.Write("koko");

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