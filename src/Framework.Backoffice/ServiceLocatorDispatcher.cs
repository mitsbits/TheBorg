﻿using Borg.Infra.Core.Log;
using Borg.Infra.CQRS;
using Borg.Infra.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice
{
    public class ServiceLocatorDispatcher : IDispatcherInstance, IEventBus, ICommandBus, IQueryBus
    {
        private readonly IServiceProvider _container;
        private ILogger Logger { get; }

        public ServiceLocatorDispatcher(IServiceProvider container)
        {
            _container = container;
            ILoggerFactory loggerFactory = (ILoggerFactory)_container.GetService(typeof(ILoggerFactory));
            if (loggerFactory != null)
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

            using (var scope = _container.CreateScope())
            {
                var type = typeof(IHandlesCommand<>).MakeGenericType(command.GetType());
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var hit = scope.ServiceProvider.GetService(collectionType);

                if (hit == null)
                {
                    Logger.LogWarning("No command handler for {Command}", command);
                    return await Task.FromResult(CommandResult.Create(false, $"no command precessor for {nameof(command)}"));
                }
                var collection = hit as IEnumerable<dynamic>;
                if (collection == null)
                {
                    Logger.LogWarning("No command handler for {Command}", command);
                    return await Task.FromResult(CommandResult.Create(false, $"no command precessor for {nameof(command)}"));
                }
                if (collection.Count() > 1)
                {
                    Logger.LogWarning("Multiple command handlers for {Command}, {@Handlers}", command, collection);
                    return await Task.FromResult(CommandResult.Create(false, $"multiple command precessors for {nameof(command)}"));
                }
                var handler = collection.Single();
                Type handlerType = handler.GetType();
                Logger.LogDebug("Found {Handler} for {Command}", handlerType, command);
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
            using (var scope = _container.CreateScope())
            {
                var tasks = new List<Task>();
                var type = typeof(IHandlesEvent<>).MakeGenericType(@event.GetType());
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var hit = scope.ServiceProvider.GetService(collectionType);
                if (hit == null)
                {
                    Logger.LogDebug("No subscribers event {Event}", @event);
                    return;
                }

                var collection = hit as IEnumerable<dynamic>;

                foreach (var handler in collection)
                {
                    var handler1 = handler;
                    Task task = handler1.Handle(@event);
                    tasks.Add(task);
                }
                Logger.LogDebug("Publishing {Event} to {@Subscribers}", @event, collection);
                await Task.WhenAll(tasks.ToArray());
            }
        }

        #endregion IEventBus



        #region IQueryBus
        public async Task<IQueryResult> Fetch<T>(T request) where T : IQueryRequest
        {
            Logger.LogDebug("Requesting handler for {@request}", request);
            IQueryResult result;

            using (var scope = _container.CreateScope())
            {
                var type = typeof(IHandlesQueryRequest<>).MakeGenericType(request.GetType());
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                var hit = scope.ServiceProvider.GetService(collectionType);

                if (hit == null)
                {
                    Logger.LogWarning("No command handler for {request}", request);
                    return await Task.FromResult(new FailQueryResult<T>());
                }
                var collection = hit as IEnumerable<dynamic>;
                if (collection == null)
                {
                    Logger.LogWarning("No command handler for {request}", request);
                    return await Task.FromResult(new FailQueryResult<T>( $"no request handler for {nameof(request)}"));
                }
                if (collection.Count() > 1)
                {
                    Logger.LogWarning("Multiple request handlers for {request}, {@Handlers}", request, collection);
                    return await Task.FromResult(new FailQueryResult<T>($"multiple request handlers for {nameof(request)}"));
                }
                var handler = collection.Single();
                Type handlerType = handler.GetType();
                Logger.LogDebug("Found {Handler} for {Command}", handlerType, request);
                Task<IQueryResult> task = handler.Execute(request);

                result = await task;
            }

            return result;
        } 
        #endregion

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
                //_container.Dispose();
            }
        }



        #endregion IDisposable
    }
}