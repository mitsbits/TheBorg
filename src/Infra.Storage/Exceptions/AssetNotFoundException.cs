using System;

namespace Borg.Infra.Storage
{
    public class AssetNotFoundException<TKey> : Exception
    {
        public AssetNotFoundException(TKey id) : base($"Asset not found for {id}") {}
    }
}