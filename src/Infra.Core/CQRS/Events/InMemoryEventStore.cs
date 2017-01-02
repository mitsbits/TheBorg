using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public class InMemoryEventStore<TKey> : IDomainEventStore<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IEventBus _publisher;

        private readonly Dictionary<Type, Dictionary<TKey, List<IDomainEvent<TKey>>>> _inMemoryDb =
            new Dictionary<Type, Dictionary<TKey, List<IDomainEvent<TKey>>>>();

        public InMemoryEventStore(IEventBus publisher)
        {
            _publisher = publisher;
        }

        public InMemoryEventStore()
        {
        }

        public IEnumerable<TKey> AggregateKeys => _inMemoryDb.SelectMany(x => x.Value.Keys).Distinct();

        public void Clear()
        {
            _inMemoryDb.Clear();
        }

        public async Task Save<T>(IEnumerable<IDomainEvent<TKey>> events)
        {
            foreach (var @event in events)
            {
                Dictionary<TKey, List<IDomainEvent<TKey>>> partition;
                if (_inMemoryDb.TryGetValue(typeof(T), out partition))
                {
                    List<IDomainEvent<TKey>> list;
                    partition.TryGetValue(@event.Id, out list);
                    if (list == null)
                    {
                        list = new List<IDomainEvent<TKey>>();
                        partition.Add(@event.Id, list);
                    }
                    list.Add(@event);
                }
                else
                {
                    partition = new Dictionary<TKey, List<IDomainEvent<TKey>>>
                    {
                        {@event.Id, new List<IDomainEvent<TKey>> {@event}}
                    };
                    _inMemoryDb.Add(typeof(T), partition);
                }
                if (_publisher != null) await _publisher.Publish(@event);
            }
        }

        public Task<IEnumerable<IDomainEvent<TKey>>> Get<T>(TKey aggregateId, int fromVersion = -1)
        {
            Dictionary<TKey, List<IDomainEvent<TKey>>> partition;
            List<IDomainEvent<TKey>> events = null;
            var result = new List<IDomainEvent<TKey>>();
            _inMemoryDb.TryGetValue(typeof(T), out partition);
            partition?.TryGetValue(aggregateId, out events);
            return Task.FromResult(events?.Where(x => x.Version > fromVersion) ?? new List<IDomainEvent<TKey>>());
        }
    }
}