using System;

namespace Infra.Core.Caching
{
    public interface ICanProduceCacheKey
    {
        string CacheKey { get; }
    }

    public interface ICanProduceCacheExpiresIn
    {
        TimeSpan? ExpiresIn { get; }
    }
}