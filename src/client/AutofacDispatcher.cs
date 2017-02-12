using Autofac;
using Borg.Infra.Core.Log;
using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Client
{
    public class AutofacDispatcher : IDispatcherInstance, IEventBus, ICommandBus
    {
        private readonly ILifetimeScope _container;
        private ILogger Logger { get; }

        public AutofacDispatcher(ILifetimeScope container)
        {
            _container = container;
            ILoggerFactory loggerFactory;
            if (_container.TryResolve(out loggerFactory))
            {
                Logger = loggerFactory.CreateLogger(GetType());
            }
            else
            {
                Logger = new SuppressLogger();
            }
        }

        #region ICommandBus

        public async Task<ICommandResult> Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            Logger.LogDebug("Requesting handler for {@Command}", command);
            ICommandResult result;

            using (var scope = _container.BeginLifetimeScope())
            {
                var type = typeof(IHandlesCommand<>).MakeGenericType(command.GetType());
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var hit = scope.Resolve(collectionType);

                if (hit == null)
                {
                    Logger.LogWarning("No command handler for {@Command}", command);
                    return await Task.FromResult(CommandResult.Create(false, $"no command precessor for {nameof(command)}"));
                }
                var collection = hit as IEnumerable<dynamic>;
                if (collection == null)
                {
                    Logger.LogWarning("No command handler for {@Command}", command);
                    return await Task.FromResult(CommandResult.Create(false, $"no command precessor for {nameof(command)}"));
                }
                if (collection.Count() > 1)
                {
                    Logger.LogWarning("Multiple command handlers for {@Command}, {@Handlers}", command, collection);
                    return await Task.FromResult(CommandResult.Create(false, $"multiple command precessors for {nameof(command)}"));
                }
                var handler = collection.Single();
                Type handlerType = handler.GetType();
                Logger.LogDebug("Found {Handler} for {@Command}", handlerType, command);
                Task<ICommandResult> task = handler.Execute(command);

                result = await task;
            }

            return result;
        }

        #endregion ICommandBus

        #region IEventBus

        public async Task Publish<T>(T @event) where T : IEvent
        {
            Logger.LogDebug("Publishing event {@Event}", @event);
            using (var scope = _container.BeginLifetimeScope())
            {
                var tasks = new List<Task>();
                var type = typeof(IHandlesEvent<>).MakeGenericType(@event.GetType());
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var hit = scope.Resolve(collectionType);
                if (hit == null)
                {
                    Logger.LogDebug("No subscribers event {@Event}", @event);
                    return;
                }

                var collection = hit as IEnumerable<dynamic>;

                foreach (var handler in collection)
                {
                    var handler1 = handler;
                    Task task = handler1.Handle(@event);
                    tasks.Add(task);
                }
                Logger.LogDebug("Publishing {@Event} to {@Subscribers}", @event, collection);
                await Task.WhenAll(tasks.ToArray());
            }
        }

        #endregion IEventBus

        #region IDispatcherInstance

        public Task Stop(CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();
            Logger.LogDebug("Stoping {Dispatcher}", GetType());
            return Task.FromResult(0);
        }

        #endregion IDispatcherInstance

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _container.Dispose();
            }
        }

        #endregion IDisposable
    }
}