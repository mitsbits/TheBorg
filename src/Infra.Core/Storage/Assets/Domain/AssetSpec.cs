using System;
using System.Collections.Generic;

namespace Borg.Infra.Storage.Assets
{
    internal class AssetSpec<TKey> : AssetSpec, IAssetSpec<TKey> where TKey : IEquatable<TKey>
    {
        public AssetSpec(TKey id, IAssetSpec fileSpec) : this(id, fileSpec.State, fileSpec.CurrentFile, fileSpec.Name)
        {
        }

        public AssetSpec(TKey id, AssetState state, IVersionSpec currentFile, string name) : base(state, currentFile, name)
        {
            Id = id;
        }

        public TKey Id { get; }
    }

    internal class AssetSpec : IAssetSpec
    {
        public AssetSpec(AssetState state, IVersionSpec currentFile, string name)
        {
            State = state;
            CurrentFile = currentFile;
            Name = name;
        }

        public AssetState State { get; protected set; }
        public IVersionSpec CurrentFile { get; protected set; }
        public string Name { get; protected set; }

        public virtual void Activate()
        {
            if (State == AssetState.Active) return; State = AssetState.Active;
        }

        public virtual void Deactivate()
        {
            if (State == AssetState.Suspended) return; State = AssetState.Suspended;
        }

        public ICollection<IVersionSpec> Versions { get; set; } = new HashSet<IVersionSpec>();
        IEnumerable<IVersionSpec> IAssetSpec.Versions => Versions;
    }
}