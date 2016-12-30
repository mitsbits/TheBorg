using System;

namespace Borg.Infra.Storage
{
    public class AssetVersionsNotFoundException<TKey> : Exception
    {
        public AssetVersionsNotFoundException(TKey id) : base($"Asset Versions not found for {id}") { }
    }
}