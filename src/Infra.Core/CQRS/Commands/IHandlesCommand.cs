using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IHandlesCommand<in TCommand> where TCommand : ICommand
    {
        Task<ICommandResult> Execute(TCommand message);
    }
}