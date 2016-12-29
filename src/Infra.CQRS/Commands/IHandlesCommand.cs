using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IHandlesCommand<in T> where T : ICommand
    {
        Task<ICommandResult> Execute(T message);
    }
}