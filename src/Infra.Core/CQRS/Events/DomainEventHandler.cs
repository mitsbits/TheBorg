using System;
using System.Threading.Tasks;

namespace Borg.Infra.CQRS
{
    public abstract class DomainEventHandler<T, TKey> : IHandlesEvent<T> where T : IDomainEvent<TKey> where TKey : IEquatable<TKey>
    {
        public abstract Task Handle(T message);
    }
}