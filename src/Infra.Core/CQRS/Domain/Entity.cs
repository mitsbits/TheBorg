using Borg.Infra.BuildingBlocks;
using System;

namespace Borg.Infra.CQRS
{
    public abstract class Entity<TKey> : IEntity<TKey>, IHavePartitionedKey where TKey : IEquatable<TKey>
    {
        protected Entity()
        {
            Id = default(TKey);
        }

        protected Entity(TKey id)
        {
            Id = id;
        }

        public TKey Id { get; private set; }

        private PartitionedKey _partitionKeyValue;
        PartitionedKey IHavePartitionedKey.Key => _partitionKeyValue ?? (_partitionKeyValue = new PartitionedKey(Id.ToString()));

        protected void SetId(TKey id)
        {
            if (id == null || id.Equals(default(TKey))) return;
            if (Id.Equals(id)) return;
            Id = id;
            _partitionKeyValue = null;
        }
    }
}