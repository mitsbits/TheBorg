using System;
using System.Linq;
using System.Reflection;

namespace Borg.Infra.CQRS
{
    public class IntervalSnapshotStrategy : ISnapshotStrategy
    {
        private readonly int SnapshotInterval;

        public IntervalSnapshotStrategy(int snapshotInterval)
        {
            SnapshotInterval = snapshotInterval;
        }

        public bool IsSnapshotable(Type aggregateType)
        {
            if (aggregateType.GetTypeInfo().BaseType == null)
            {
                return false;
            }
            var memberInfo = aggregateType.GetTypeInfo().BaseType;
            if (memberInfo != null && memberInfo.GetTypeInfo().IsGenericType
                && memberInfo.GetGenericTypeDefinition().GetInterfaces().Any(x => x.IsGenericType
                && x.GetGenericTypeDefinition() == typeof(ISnapshotAggregateRoot<,>)))
            {
                return true;
            }
            return IsSnapshotable(memberInfo);
        }

        public bool ShouldMakeSnapShot<TKey>(AggregateRoot<TKey> aggregate) where TKey : IEquatable<TKey>
        {
            if (!IsSnapshotable(aggregate.GetType()))
            {
                return false;
            }
            var i = aggregate.Version;

            for (var j = 0; j < aggregate.GetUncommittedChanges().Count(); j++)
            {
                if (++i % SnapshotInterval == 0 && i != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}