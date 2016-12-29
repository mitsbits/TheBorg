using System;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public class AggregateRepository<TKey> : IAggregateRepository<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IDomainEventStore<TKey> _eventStore;
        private readonly IEventBus _publisher;

        public AggregateRepository(IDomainEventStore<TKey> eventStore)
        {
            if (eventStore == null)
            {
                throw new ArgumentNullException(nameof(eventStore));
            }

            _eventStore = eventStore;
        }

        //[Obsolete("The eventstore should publish events after saving")]
        public AggregateRepository(IDomainEventStore<TKey> eventStore, IEventBus publisher)
        {
            if (eventStore == null)
            {
                throw new ArgumentNullException(nameof(eventStore));
            }
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }
            _eventStore = eventStore;
            _publisher = publisher;
        }

        public async Task Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot<TKey>
        {
            if (expectedVersion != null && (await _eventStore.Get<T>(aggregate.Id, expectedVersion.Value)).Any())
            {
                throw new ConcurrencyException<TKey>(aggregate.Id);
            }

            var changes = aggregate.FlushUncommitedChanges();
            await _eventStore.Save<T>(changes);

            if (_publisher != null)
            {
                foreach (var @event in changes)
                {
                    await _publisher.Publish(@event);
                }
            }
        }

        public Task<T> Get<T>(TKey aggregateId) where T : AggregateRoot<TKey>
        {
            return LoadAggregate<T>(aggregateId);
        }

        private async Task<T> LoadAggregate<T>(TKey id) where T : AggregateRoot<TKey>
        {
            var events = await _eventStore.Get<T>(id, -1);
            if (!events.Any())
            {
                throw new AggregateNotFoundException<TKey>(typeof(T), id);
            }

            var aggregate = AggregateFactory.CreateAggregate<T>();
            aggregate.LoadFromHistory(events);
            return aggregate;
        }

        //TODO: make factory plugable
        internal static class AggregateFactory
        {
            public static T CreateAggregate<T>()
            {
                try
                {
                    return (T)Activator.CreateInstance(typeof(T), true);
                }
                catch (MissingMethodException)
                {
                    throw new MissingParameterLessConstructorException(typeof(T));
                }
            }
        }
    }
}