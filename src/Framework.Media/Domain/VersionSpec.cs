using Borg.Infra.Storage;
using System.ComponentModel.DataAnnotations;
using Borg.Infra.Storage.Assets;

namespace Borg.Framework.Media
{
    public class VersionSpec : IVersionSpec
    {
        public VersionSpec()
        {
        }

        [Key]
        public int Id { get; set; }

        public int AssetId { get; set; }
        public virtual AssetSpec Asset { get; set; }
        public int Version { get; set; }
        public FileSpec FileSpec { get; set; }
        IFileSpec IVersionSpec.FileSpec => FileSpec;
    }
}