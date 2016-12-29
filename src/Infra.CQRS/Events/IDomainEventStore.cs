using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public interface IDomainEventStore<TKey> where TKey : IEquatable<TKey>
    {
        Task Save<T>(IEnumerable<IDomainEvent<TKey>> events);

        Task<IEnumerable<IDomainEvent<TKey>>> Get<T>(TKey aggregateId, int fromVersion);
    }
}