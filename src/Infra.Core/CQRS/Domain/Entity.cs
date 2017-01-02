using System;

namespace Borg.Infra.CQRS
{
    public abstract class Entity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
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

        protected void SetId(TKey id)
        {
            if (Id != null && Id.Equals(id)) return;
            Id = id;
        }
    }
}