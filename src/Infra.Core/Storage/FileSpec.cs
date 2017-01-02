using System;

namespace Borg.Infra.Storage
{
    public class FileSpec : IFileSpec
    {
        public FileSpec(string fullPath, string name, DateTime creationDate, DateTime lastWrite, DateTime? lastRead, long sizeInBytes, string mimeType = "")
        {
            FullPath = fullPath;
            Name = name;
            CreationDate = creationDate;
            LastWrite = lastWrite;
            LastRead = lastRead;
            SizeInBytes = sizeInBytes;
            MimeType = string.IsNullOrWhiteSpace(mimeType) ? fullPath.GetMimeType() : mimeType;
        }

        public string FullPath { get; protected set; }
        public string Name { get; }
        public DateTime CreationDate { get; }
        public DateTime LastWrite { get; protected set; }
        public DateTime? LastRead { get; }
        public long SizeInBytes { get; }
        public string MimeType { get; }

        public void ModifyPath(string newPath)
        {
            FullPath = newPath;
            LastWrite = DateTime.UtcNow;
        }
    }
}