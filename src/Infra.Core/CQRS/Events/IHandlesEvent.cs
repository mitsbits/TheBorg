using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IHandlesEvent<in T> where T : IEvent
    {
        Task Handle(T message);
    }
}