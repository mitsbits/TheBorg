using System;

namespace Borg.Infra.CQRS
{
    public interface ISnapshotStrategy
    {
        bool ShouldMakeSnapShot<TKey>(AggregateRoot<TKey> aggregate) where TKey : IEquatable<TKey>;

        bool IsSnapshotable(Type aggregateType);
    }
}