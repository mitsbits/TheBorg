using Borg.Infra.BuildingBlocks;

namespace Borg.Infra.Caching
{
    public interface ICanProduceCacheKey
    {
        string CacheKey { get; }
    }

    public interface ICacheDependency<in TEntity>
    {
        PartitionedKey Key { get; }
    }
}