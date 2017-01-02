using System;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public class SnapshotRepository<TKey, TSnapshot> : IAggregateRepository<TKey> where TKey : IEquatable<TKey> where TSnapshot : ISnapshot<TKey, IAggregateRoot<TKey>>
    {
        private readonly ISnapshotStore<TKey, TSnapshot> _snapshotStore;
        private readonly ISnapshotStrategy _snapshotStrategy;
        private readonly IAggregateRepository<TKey> _repository;
        private readonly IDomainEventStore<TKey> _eventStore;

        public SnapshotRepository(ISnapshotStore<TKey, TSnapshot> snapshotStore, ISnapshotStrategy snapshotStrategy, IAggregateRepository<TKey> repository, IDomainEventStore<TKey> eventStore)
        {
            if (snapshotStore == null)
            {
                throw new ArgumentNullException(nameof(snapshotStore));
            }
            if (snapshotStrategy == null)
            {
                throw new ArgumentNullException(nameof(snapshotStrategy));
            }
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            if (eventStore == null)
            {
                throw new ArgumentNullException(nameof(eventStore));
            }

            _snapshotStore = snapshotStore;
            _snapshotStrategy = snapshotStrategy;
            _repository = repository;
            _eventStore = eventStore;
        }

        public Task Save<T>(T aggregate, int? exectedVersion = null) where T : AggregateRoot<TKey>
        {
            TryMakeSnapshot<T>(aggregate);
            _repository.Save(aggregate, exectedVersion);
            return Task.CompletedTask;
        }

        public async Task<T> Get<T>(TKey aggregateId) where T : AggregateRoot<TKey>
        {
            var aggregate = AggregateRepository<TKey>.AggregateFactory.CreateAggregate<T>();
            var snapshotVersion = await TryRestoreAggregateFromSnapshot(aggregateId, aggregate);
            if (snapshotVersion == -1)
            {
                return await _repository.Get<T>(aggregateId);
            }
            var events = (await _eventStore.Get<T>(aggregateId, snapshotVersion)).Where(desc => desc.Version > snapshotVersion);
            aggregate.LoadFromHistory(events);

            return aggregate;
        }

        private async Task<int> TryRestoreAggregateFromSnapshot<T>(TKey id, T aggregate) where T : AggregateRoot<TKey>
        {
            var version = -1;
            if (_snapshotStrategy.IsSnapshotable(typeof(T)))
            {
                var snapshot = await _snapshotStore.Get<T>(id);
                if (snapshot != null)
                {
                    aggregate.AsDynamic().Restore(snapshot);
                    version = snapshot.Version;
                }
            }
            return version;
        }

        private void TryMakeSnapshot<T>(AggregateRoot<TKey> aggregate) where T : AggregateRoot<TKey>
        {
            if (!_snapshotStrategy.ShouldMakeSnapShot(aggregate))
            {
                return;
            }
            var snapshot = aggregate.AsDynamic().GetSnapshot().RealObject;
            snapshot.Version = aggregate.Version + aggregate.GetUncommittedChanges().Count();
            _snapshotStore.Save<T>(snapshot);
        }
    }
}