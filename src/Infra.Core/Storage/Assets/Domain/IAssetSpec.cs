using System;
using System.Collections.Generic;

namespace Borg.Infra.Storage.Assets
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

        IEnumerable<IVersionSpec> Versions { get; }
    }
}