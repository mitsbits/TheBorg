using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public class InMemoryRetriever : IQueryBus
    {
        private readonly Dictionary<Type, List<Func<IQueryRequest, IResponse>>> _routes = new Dictionary<Type, List<Func<IQueryRequest, IResponse>>>();

        public void RegisterHandler<T>(Func<T, IResponse> handler) where T : IQueryRequest
        {
            List<Func<IQueryRequest, IResponse>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Func<IQueryRequest, IResponse>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(DelegateAdjuster.CastArgument<IQueryRequest, T>(x => handler(x)));
        }

        private Task<IQueryResult> Process<TQuery>(TQuery query) where TQuery : IQueryRequest
        {
            List<Func<IQueryRequest, IResponse>> handlers;
            if (_routes.TryGetValue(typeof(IQueryRequest), out handlers))
            {
                if (!handlers.Any()) throw new NoRetrieversForQueryException(query.GetType());
                if (handlers.Count > 1) throw new MultipleRetrieversForQueryException(query.GetType());
                var handler1 = handlers[0];
                return Task.FromResult(handler1.Invoke(query) as IQueryResult);
            }
            else
            {
                throw new NoRetrieversForQueryException(query.GetType());
            }
        }

        public async Task<IQueryResult<V>> Fetch<T, V>(T request) where T : IQueryRequest where V : IResponse
        {
            return await Process(request) as IQueryResult<V>;
        }
    }
}