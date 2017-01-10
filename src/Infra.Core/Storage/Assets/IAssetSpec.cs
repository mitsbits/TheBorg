using System;

namespace Borg.Infra.Storage
{
    public interface IAssetSpec<out TKey> : IAssetSpec where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }

    public interface IAssetSpec
    {
        AssetState State { get; }
        IVersionSpec CurrentFile { get; }
        string Name { get; }
    }
}