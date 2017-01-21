using Borg.Infra.BuildingBlocks;
using System;

namespace Borg.Infra.Caching
{
    public interface IEntityCacheDepedencyEvictionEvent
    {
        PartitionedKey[] Keys { get; }
        Type Type { get; }
    }

    public class EntityCacheDepedencyEvictionEvent : IEntityCacheDepedencyEvictionEvent
    {
        public EntityCacheDepedencyEvictionEvent(Type type, PartitionedKey[] keys) : this(type.AssemblyQualifiedName, keys)
        {
        }

        protected EntityCacheDepedencyEvictionEvent(string type, PartitionedKey[] keys)
        {
            Type = type;
            Keys = keys;
        }

        public PartitionedKey[] Keys { get; }
        public string Type { get; }

        Type IEntityCacheDepedencyEvictionEvent.Type => System.Type.GetType(Type);
    }
}