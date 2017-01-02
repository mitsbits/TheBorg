using System;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface ISnapshotStore<in TKey, TSnapshot> where TKey : IEquatable<TKey> where TSnapshot : ISnapshot<TKey, IAggregateRoot<TKey>>
    {
        Task<TSnapshot> Get<T>(TKey id) where T : IAggregateRoot<TKey>;

        Task Save<T>(TSnapshot snapshotBase) where T : IAggregateRoot<TKey>;
    }
}