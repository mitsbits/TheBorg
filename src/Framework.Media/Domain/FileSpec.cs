using Borg.Infra.Storage;
using System;
using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.Media
{
    public class FileSpec : IFileSpec<int>
    {
        public FileSpec()
        {
        }

        public int VersionId { get; set; }
        public virtual VersionSpec Version { get; set; }

        public string FullPath { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastWrite { get; set; }
        public DateTime? LastRead { get; set; }
        public long SizeInBytes { get; set; }
        public string MimeType { get; set; }

        public void ModifyPath(string newPath)
        {
            if (FullPath == newPath) return;
            FullPath = newPath;
        }

        [Key]
        public int Id { get; protected set; }
    }
}