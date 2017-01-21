using Borg.Infra.BuildingBlocks;
using System.Threading.Tasks;

namespace Borg.Infra.Caching
{
    public interface IDepedencyCacheClient : ICacheClient, ICacheDepedencyManager { }

    public static class ICacheDepedencyManagerExtensions
    {
        public static Task Add<TEntity>(this ICacheDepedencyManager manager, string key, params PartitionedKey[] identifiers)
        {
            return manager.Add(key, typeof(TEntity), identifiers);
        }

        public static Task Invalidate<TEntity>(this ICacheDepedencyManager manager, params PartitionedKey[] identifiers)
        {
            return manager.Invalidate(typeof(TEntity), identifiers);
        }
    }
}