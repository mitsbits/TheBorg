using System;
using Borg.Infra.CQRS;
using Borg.Infra.Storage;

namespace Borg.Infra.Core.Storage.Assets
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
        public DateTimeOffset TimeStamp { get; } = DateTimeOffset.UtcNow;
    }
}