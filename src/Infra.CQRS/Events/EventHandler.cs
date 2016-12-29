using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public abstract class EventHandler<T> : IHandlesEvent<T> where T : IEvent
    {
        public abstract Task Handle(T message);
    }
}