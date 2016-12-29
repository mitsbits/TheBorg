using System;

namespace Borg.Infra.CQRS
{
    public abstract class SnapshotAggregateRoot<TKey, TSnapshot> : AggregateRoot<TKey>, ISnapshotAggregateRoot<TKey, TSnapshot> where TKey : IEquatable<TKey> where TSnapshot : ISnapshot<TKey, IAggregateRoot<TKey>>
    {
        public TSnapshot GetSnapshot()
        {
            var snapshot = CreateSnapshot();
            return snapshot;
        }

        public void Restore(TSnapshot snapshot)
        {
            RestoreFromSnapshot(snapshot);
            SetId(snapshot.Id);
            Version = snapshot.Version;
        }

        protected abstract TSnapshot CreateSnapshot();

        protected abstract void RestoreFromSnapshot(TSnapshot snapshot);
    }
}