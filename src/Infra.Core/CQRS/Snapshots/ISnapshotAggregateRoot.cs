using System;

namespace Borg.Infra.CQRS
{
    public interface ISnapshotAggregateRoot<TKey, TSnapshot> where TKey : IEquatable<TKey> where TSnapshot : ISnapshot<TKey, IAggregateRoot<TKey>>
    {
        TSnapshot GetSnapshot();

        void Restore(TSnapshot snapshot);
    }
}