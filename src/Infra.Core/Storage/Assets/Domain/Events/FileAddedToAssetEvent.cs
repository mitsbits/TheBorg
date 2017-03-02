using Borg.Infra.CQRS;
using System;

namespace Borg.Infra.Storage.Assets
{
    public class FileAddedToAssetEvent<TKey> : IEvent where TKey : IEquatable<TKey>
    {
        public FileAddedToAssetEvent(IFileSpec file, IAssetSpec<TKey> asset)
        {
            File = file;
            Asset = asset;
        }

        public IFileSpec File { get; }
        public IAssetSpec<TKey> Asset { get; }
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
    }
}