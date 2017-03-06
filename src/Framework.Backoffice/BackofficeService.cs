using System;
using System.Threading;
using System.Threading.Tasks;
using Borg.Framework.System;
using Borg.Infra;
using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using Microsoft.Extensions.Logging;

namespace Borg.Framework.Backoffice
{
    public class BackofficeService : SystemService, IBackofficeService<BorgSettings>
    {
        public BackofficeService( IBorgHost  borgHost, ILoggerFactory loggerFactory, BorgSettings settings, ISerializer serializer, ICommandBus commands, IEventBus events, IQueryBus queries, IBroadcaster broadcaster) 
            : base(borgHost, loggerFactory,  settings,  serializer)
        {
            Commands = commands;
            Events = events;
            Queries = queries;
            Broadcaster = broadcaster;
        }

        public IBroadcaster Broadcaster { get; }

        public ICommandBus Commands { get; }

        public IEventBus Events { get; }

        public IQueryBus Queries { get; }

        public Task Broadcast(string[] topics, Type messageType, object message, TimeSpan? delay = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return Broadcaster.Broadcast(topics, messageType, message, delay, cancellationToken);
        }

        public Task<ICommandResult> Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            return Commands.Process(command);
        }

        public Task Publish<T>(T @event) where T : IEvent
        {
            return Events.Publish(@event);
        }

        public Task<IQueryResult> Fetch<T>(T request) where T : IQueryRequest
        {
            return Queries.Fetch(request);
        }
    }
}