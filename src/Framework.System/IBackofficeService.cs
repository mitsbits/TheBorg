using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using Borg.Infra.Postal;

namespace Borg.Framework.System
{
    public interface IBackofficeService<out TSettings> : ISystemService<TSettings>
        , IEventBus, ICommandBus, IQueryBus, IBroadcaster, IEmailAccountService where TSettings : BorgSettings
    {
        IEventBus Events { get; }
        ICommandBus Commands { get; }
        IQueryBus Queries { get; }
        IBroadcaster Broadcaster { get; }
        IEmailAccountService Emails { get; }
    }
}