using Borg.Infra.CQRS;
using Borg.Infra.Messaging;

namespace Borg.Framework.System
{
    public interface IBackofficeService<out TSettings> : ISystemService<TSettings>
        , IEventBus, ICommandBus, IQueryBus, IBroadcaster where TSettings : BorgSettings
    {
        IEventBus Events { get; }
        ICommandBus Commands { get; }
        IQueryBus Queries { get; }
        IBroadcaster Broadcaster { get; }
    }
}