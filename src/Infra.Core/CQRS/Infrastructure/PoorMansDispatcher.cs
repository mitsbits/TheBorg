using Borg.Infra.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public class PoorMansDispatcher : IDispatcherInstance, IEventBus, ICommandBus, IQueryBus
    {
        private readonly IServiceProvider _provider;

        public PoorMansDispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        #region ICommandBus

        public async Task<ICommandResult> Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            var type = typeof(IHandlesCommand<>).MakeGenericType(command.GetType());
            var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
            var hit = _provider.GetService(collectionType);
            if (hit == null) return await Task.FromResult(CommandResult.Create(false,
                $"no command precessor for {nameof(command)}"));

            var collection = hit as IEnumerable<dynamic>;
            var handlers = collection as dynamic[] ?? collection.ToArray();
            if (handlers.Count() > 1) return await Task.FromResult(CommandResult.Create(false,
                $"multiple command precessors for {nameof(command)}"));
            var handler = handlers.Single();

            Task<ICommandResult> task = handler.Execute(command);

            return await task;
        }

        #endregion ICommandBus

        #region IEventBus

        public async Task Publish<T>(T @event) where T : IEvent
        {
            var type = typeof(IHandlesEvent<>).MakeGenericType(@event.GetType());
            var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
            var hit = _provider.GetService(collectionType);
            if (hit == null) return;

            var collection = hit as IEnumerable<dynamic>;
            var tasks = new List<Task>();
            if (collection != null)
                foreach (var handler in collection)
                {
                    Task task = handler.Handle(@event);
                    tasks.Add(task);
                }
            await Task.WhenAll(tasks.ToArray());
        }

        #endregion IEventBus

        #region IQueryBus

        public async Task<IQueryResult<V>> Fetch<T, V>(T request) where T : IQueryRequest where V : IResponse
        {
            var type = typeof(IHandlesQueryRequest<>).MakeGenericType(request.GetType());
            var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
            var hit = _provider.GetService(collectionType);
            if (hit == null) return await Task.FromResult(new FailQueryResult<V>($"no request handler for {nameof(request)}"));

            var collection = hit as IEnumerable<dynamic>;
            var handlers = collection as dynamic[] ?? collection.ToArray();
            if (handlers.Count() > 1) return await Task.FromResult(new FailQueryResult<V>($"multiple request handlers for {nameof(request)}"));
            var handler = handlers.Single();
            Task<IQueryResult<V>> task = handler.Execute(request);
            return await task;
        }

        #endregion IQueryBus

        #region IDispatcherInstance

        public Task Stop(CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //release resources
            }
        }

        public Task<IQueryResult> Fetch<T>(T request) where T : IQueryRequest
        {
            throw new NotImplementedException();
        }

        #endregion IDispatcherInstance
    }
}