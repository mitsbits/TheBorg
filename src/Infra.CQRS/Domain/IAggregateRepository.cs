using System;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IAggregateRepository<TKey> where TKey : IEquatable<TKey>
    {
        Task Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot<TKey>;

        Task<T> Get<T>(TKey aggregateId) where T : AggregateRoot<TKey>;
    }
}