using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Borg.Infra.CQRS;
using Borg.Infra.Messaging;

namespace Borg.Client
{
    public class AutofacDispatcher : IDispatcherInstance, IEventBus, ICommandBus
    {
        //private readonly ILogger _dLog = Log.Logger.ForContext(typeof(AutofacDispatcher));
        private readonly ILifetimeScope _container;
        public AutofacDispatcher(ILifetimeScope container)
        {
            _container = container;
        }

        #region ICommandBus

        public async Task<ICommandResult> Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            //_dLog.Debug("Requesting handler for {@Command}", command);
            ICommandResult result;

            using (var scope = _container.BeginLifetimeScope())
            {
                var type = typeof(IHandlesCommand<>).MakeGenericType(command.GetType());
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var hit = scope.Resolve(collectionType);

                if (hit == null)
                {
                    // _dLog.Debug("No command handler for {@Command}", command);
                    return await Task.FromResult(CommandResult.Create(false, $"no command precessor for {nameof(command)}"));
                }
                var collection = hit as IEnumerable<dynamic>;
                if (collection == null)
                {
                    //_dLog.Debug("No command handler for {@Command}", command);
                    return await Task.FromResult(CommandResult.Create(false, $"no command precessor for {nameof(command)}"));
                }
                if (collection.Count() > 1)
                {
                    //_dLog.Debug("Multiple command handlers for {@Command}, {@Handlers}", command, collection);
                    return await Task.FromResult(CommandResult.Create(false,
                    $"multiple command precessors for {nameof(command)}"));
                }
                var handler = collection.Single();
                //_dLog.Debug("Found {@Handler} for {@Command}", handler, command);
                Task<ICommandResult> task = handler.Execute(command);

                result = await task;
            }

            // _dLog.Debug("Executing {@Handler} for {@Command} resulted in {@Result}", handler, command, result);
            return result;
        }

        #endregion ICommandBus

        #region IEventBus

        public async Task Publish<T>(T @event) where T : IEvent
        {
            // _dLog.Debug("Publishing event {@Event}", @event);
            using (var scope = _container.BeginLifetimeScope())
            {
                var tasks = new List<Task>();
                var type = typeof(IHandlesEvent<>).MakeGenericType(@event.GetType());
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var hit = scope.Resolve(collectionType);
                if (hit == null)
                {
                    // _dLog.Debug("No subscribers event {@Event}", @event);
                    return;
                }

                var collection = hit as IEnumerable<dynamic>;

                foreach (var handler in collection)
                {
                    var handler1 = handler;
                    Task task = handler1.Handle(@event);
                    tasks.Add(task);
                }
                // _dLog.Debug("Publishing {@Event} to {@Subscribers}", @event, collection);
                await Task.WhenAll(tasks.ToArray());
            }
        }

        #endregion IEventBus

        #region IDispatcherInstance

        public Task Stop(CancellationToken token = default(CancellationToken))
        {
            token.ThrowIfCancellationRequested();
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
