using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public interface IFileStorage : IDisposable
    {
        Task<Stream> GetFileStreamAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        Task<IFileSpec> GetFileInfoAsync(string path);

        Task<bool> ExistsAsync(string path);

        Task<bool> SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> RenameFileAsync(string path, string newpath, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> CopyFileAsync(string path, string targetpath, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<IFileSpec>> GetFileListAsync(string searchPattern = null, int? limit = null, int? skip = null, CancellationToken cancellationToken = default(CancellationToken));
    }

    //public class FileSpec
    //{
    //    public string Path { get; set; }
    //    public DateTime Created { get; set; }
    //    public DateTime Modified { get; set; }
    //    public long Size { get; set; }
    //    // TODO: Add metadata object for custom properties
    //}
}