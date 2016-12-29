using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public class InMemoryProcessor : ICommandBus
    {
        private readonly Dictionary<Type, List<Func<ICommand, IResponse>>> _routes = new Dictionary<Type, List<Func<ICommand, IResponse>>>();

        public void RegisterHandler<T>(Func<T, IResponse> handler) where T : ICommand
        {
            List<Func<ICommand, IResponse>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Func<ICommand, IResponse>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<ICommand, T>(x => handler(x)));
        }

        public Task<ICommandResult> Process<TCommand>(TCommand command) where TCommand : ICommand
        {
            List<Func<ICommand, IResponse>> handlers;
            if (_routes.TryGetValue(command.GetType(), out handlers))
            {
                if (!handlers.Any()) throw new NoHandlersForCommandException(command.GetType());
                if (handlers.Count > 1) throw new MultipleHandlersForCommandException(command.GetType());
                var handler1 = handlers[0];
                return Task.FromResult(handler1.Invoke(command) as ICommandResult);
            }
            else
            {
                throw new NoHandlersForCommandException(command.GetType());
            }
        }
    }
}