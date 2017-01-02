using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public class InMemorySnapshotStore<TKey, TSnapshot> : ISnapshotStore<TKey, TSnapshot> where TKey : IEquatable<TKey> where TSnapshot : ISnapshot<TKey, IAggregateRoot<TKey>>
    {
        private readonly Dictionary<Type, Dictionary<TKey, TSnapshot>> _inMemoryDb =
            new Dictionary<Type, Dictionary<TKey, TSnapshot>>();

        public IEnumerable<TKey> AggregateKeys => _inMemoryDb.SelectMany(x => x.Value.Keys).Distinct();

        public void Clear()
        {
            _inMemoryDb.Clear();
        }

        public Task<TSnapshot> Get<T>(TKey id) where T : IAggregateRoot<TKey>
        {
            Dictionary<TKey, TSnapshot> partition;
            TSnapshot result = default(TSnapshot);
            _inMemoryDb.TryGetValue(typeof(T), out partition);
            partition?.TryGetValue(id, out result);
            return Task.FromResult(result);
        }

        public Task Save<T>(TSnapshot snapshot) where T : IAggregateRoot<TKey>
        {
            Dictionary<TKey, TSnapshot> partition;
            if (_inMemoryDb.TryGetValue(typeof(T), out partition))
            {
                TSnapshot entry;
                partition.TryGetValue(snapshot.Id, out entry);

                if (entry == null)
                {
                    entry = snapshot;
                    partition.Add(snapshot.Id, entry);
                }
                else
                {
                    entry = snapshot;
                    partition[snapshot.Id] = entry;
                }
            }
            else
            {
                partition = new Dictionary<TKey, TSnapshot>()
                {
                    {
                        snapshot.Id, snapshot
                    }
                };

                _inMemoryDb.Add(typeof(T), partition);
            }
            return Task.CompletedTask;
        }
    }
}