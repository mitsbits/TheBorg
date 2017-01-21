using Borg.Infra.BuildingBlocks;
using System;

namespace Borg.Infra.Caching
{
    public interface ICacheDepedencyEvent
    {
        PartitionedKey[] Key { get; }
        Type Type { get; }
    }
}