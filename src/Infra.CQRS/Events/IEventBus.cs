using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IEventBus
    {
        Task Publish<T>(T @event) where T : IEvent;
    }
}