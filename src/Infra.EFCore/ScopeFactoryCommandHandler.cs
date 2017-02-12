using Borg.Infra.CQRS;
using System.Threading.Tasks;

namespace Borg.Infra.EFCore
{
    public abstract class ScopeFactoryCommandHandler<TCommand> : IHandlesCommand<TCommand> where TCommand : ICommand
    {
        protected ScopeFactoryCommandHandler(IDbContextScopeFactory dbContextScopeFactory)
        {
            ScopeFactory = dbContextScopeFactory;
        }

        protected IDbContextScopeFactory ScopeFactory { get; }

        public abstract Task<ICommandResult> Execute(TCommand message);
    }
}