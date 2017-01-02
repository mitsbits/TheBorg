using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface ICommandBus
    {
        Task<ICommandResult> Process<TCommand>(TCommand command) where TCommand : ICommand;
    }
}