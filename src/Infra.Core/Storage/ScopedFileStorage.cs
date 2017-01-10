using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public class ScopedFileStorage : IFileStorage
    {
        private readonly string _pathPrefix;

        public ScopedFileStorage(IFileStorage storage, string scope)
        {
            UnscopedStorage = storage;
            Scope = !string.IsNullOrWhiteSpace(scope) ? scope.Trim() : null;
            _pathPrefix = Scope != null ? string.Concat(Scope, "/") : string.Empty;
        }

        public IFileStorage UnscopedStorage { get; private set; }

        public string Scope { get; private set; }

        public Task<Stream> GetFileStreamAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            return UnscopedStorage.GetFileStreamAsync(string.Concat(_pathPrefix, path), cancellationToken);
        }

        public async Task<IFileSpec> GetFileInfoAsync(string path)
        {
            var file = await UnscopedStorage.GetFileInfoAsync(string.Concat(_pathPrefix, path)).AnyContext();
            file?.ModifyPath(file.FullPath.Substring(_pathPrefix.Length));

            return file;
        }

        public Task<bool> ExistsAsync(string path)
        {
            return UnscopedStorage.ExistsAsync(string.Concat(_pathPrefix, path));
        }

        public Task<bool> SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken = new CancellationToken())
        {
            return UnscopedStorage.SaveFileAsync(string.Concat(_pathPrefix, path), stream, cancellationToken);
        }

        public Task<bool> RenameFileAsync(string path, string newpath, CancellationToken cancellationToken = new CancellationToken())
        {
            return UnscopedStorage.RenameFileAsync(string.Concat(_pathPrefix, path), string.Concat(_pathPrefix, newpath), cancellationToken);
        }

        public Task<bool> CopyFileAsync(string path, string targetpath, CancellationToken cancellationToken = new CancellationToken())
        {
            return UnscopedStorage.CopyFileAsync(string.Concat(_pathPrefix, path), string.Concat(_pathPrefix, targetpath), cancellationToken);
        }

        public Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = new CancellationToken())
        {
            return UnscopedStorage.DeleteFileAsync(string.Concat(_pathPrefix, path), cancellationToken);
        }

        public async Task<IEnumerable<IFileSpec>> GetFileListAsync(string searchPattern = null, int? limit = null, int? skip = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (String.IsNullOrEmpty(searchPattern))
                searchPattern = "*";

            var files = (await UnscopedStorage.GetFileListAsync(string.Concat(_pathPrefix, searchPattern), limit, skip, cancellationToken).AnyContext()).ToList();
            foreach (var file in files)
                file.ModifyPath(file.FullPath.Substring(_pathPrefix.Length));

            return files;
        }

        public void Dispose()
        {
        }
    }
}