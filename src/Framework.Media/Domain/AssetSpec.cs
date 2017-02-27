using Borg.Infra.Storage;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Borg.Infra.Storage.Assets;

namespace Borg.Framework.Media
{
    public class AssetSpec : IAssetSpec<int>
    {
        public AssetSpec()
        {
            Versions = new HashSet<VersionSpec>();
        }

        public AssetState State { get; protected set; }

        public virtual VersionSpec CurrentFile
        {
            get { return Versions.OrderByDescending(x => x.Version).First(); }
        }

        [MaxLength(512)]
        public string Name { get; set; }

        [Key]
        public int Id { get; set; }

        public virtual ICollection<VersionSpec> Versions { get; protected set; }

        IEnumerable<IVersionSpec> IAssetSpec.Versions => Versions;

        IVersionSpec IAssetSpec.CurrentFile
        {
            get { return Versions.OrderByDescending(x => x.Version).First(); }
        }

        public void Activate()
        {
            if (State == AssetState.Active) return;
            State = AssetState.Active;
        }

        public void Deactivate()
        {
            if (State == AssetState.Suspended) return;
            State = AssetState.Suspended;
        }
    }
}