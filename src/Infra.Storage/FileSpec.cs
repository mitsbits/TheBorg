using System;

namespace Borg.Infra.Storage
{
    public class FileSpec<TKey> : FileSpec, IFileSpec<TKey> where TKey : IEquatable<TKey>
    {
        public FileSpec(TKey id, string fullPath, string name, DateTime creationDate, DateTime lastWrite, DateTime? lastRead, long sizeInBytes, string mimeType) : base(fullPath, name, creationDate, lastWrite, lastRead, sizeInBytes, mimeType)
        {
            Id = id;
        }

        public TKey Id { get; }
    }
}