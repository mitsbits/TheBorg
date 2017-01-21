using Borg.Infra.BuildingBlocks;
using System;
using System.Threading.Tasks;

namespace Borg.Infra.Caching
{
    public interface ICacheDepedencyManager
    {
        Task Add(string key, Type entiType, PartitionedKey[] identifiers);

        Task Invalidate(Type entiType, PartitionedKey[] identifiers);
    }
}