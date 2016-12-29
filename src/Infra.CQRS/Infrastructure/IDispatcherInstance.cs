using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IDispatcherInstance
    {
        Task Stop();
    }
}