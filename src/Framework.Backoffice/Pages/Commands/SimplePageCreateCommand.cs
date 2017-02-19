using Borg.Framework.Backoffice.Pages.Data;
using Borg.Framework.MVC.Commands;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Pages.Commands
{
    public class SimplePageCreateCommand : UserCommand
    {
        public SimplePageCreateCommand(IIdentity user) : base(user)
        {
        }

        public string Title { get; set; }
        public string Path { get; set; }
    }

    public class SimplePageCreateCommandHandler : ScopeFactoryCommandHandler<SimplePageCreateCommand>
    {
        private readonly ISystemService<BorgSettings> _system;
        private readonly ILogger Logger;

        public SimplePageCreateCommandHandler(IDbContextScopeFactory dbContextScopeFactory, ISystemService<BorgSettings> system) : base(dbContextScopeFactory)
        {
            _system = system;
            Logger = _system.CreateLogger(GetType());
        }

        public override async Task<ICommandResult> Execute(SimplePageCreateCommand message)
        {
            Logger.LogDebug("{User} invoking {Handler} for {@Command}", message.User.Name, this.GetType(), message);
            using (var db = ScopeFactory.Create())
            {
                var repo = ScopeFactory.CreateRepo<ICRUDRespoditory<SimplePage>>();
                var page = new SimplePage() { Title = message.Title, Path = message.Path };
                await repo.CreateAsync(page);
                await db.SaveChangesAsync();
                return CommandResult<SimplePage>.Create(true, page);
            }
        }
    }
}