using System;

namespace Borg.Infra.Caching
{
    public interface ICanProduceCacheExpiresIn
    {
        TimeSpan? ExpiresIn { get; }
    }
}