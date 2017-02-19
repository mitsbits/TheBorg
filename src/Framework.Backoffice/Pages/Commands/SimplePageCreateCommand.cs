using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Borg.Framework.Backoffice.Pages.Data;
using Borg.Framework.System;
using Borg.Infra.CQRS;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Backoffice.Pages.Commands
{
    public class SimplePageCreateCommand : ICommand
    {
        public string Title { get;  set; }
        public string Path { get;  set; }
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
            using (var db = ScopeFactory.Create())
            {
                var repo = ScopeFactory.CreateRepo<ICRUDRespoditory<SimplePage>>();
                var page = new SimplePage() {Title = message.Title, Path = message.Path};
                await repo.CreateAsync(page);
                await db.SaveChangesAsync();
                return CommandResult<SimplePage>.Create(true, page);
            }
        }
    }
}
