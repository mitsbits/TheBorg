using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public class InMemoryFileStorage : IFileStorage
    {
        private readonly Dictionary<string, Tuple<IFileSpec, byte[]>> _storage = new Dictionary<string, Tuple<IFileSpec, byte[]>>(StringComparer.OrdinalIgnoreCase);
        private readonly object _lock = new object();

        public InMemoryFileStorage() : this(1024 * 1024 * 256, 100)
        {
        }

        public InMemoryFileStorage(long maxFileSize, int maxFiles)
        {
            MaxFileSize = maxFileSize;
            MaxFiles = maxFiles;
        }

        public long MaxFileSize { get; set; }
        public long MaxFiles { get; set; }

        public Task<Stream> GetFileStreamAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            lock (_lock)
            {
                if (!_storage.ContainsKey(path))
                    return Task.FromResult<Stream>(null);

                return Task.FromResult<Stream>(new MemoryStream(_storage[path].Item2));
            }
        }

        public async Task<IFileSpec> GetFileInfoAsync(string path)
        {
            return await ExistsAsync(path).AnyContext() ? _storage[path].Item1 : null;
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(_storage.ContainsKey(path));
        }

        private static byte[] ReadBytes(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public Task<bool> SaveFileAsync(string path, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            byte[] contents = ReadBytes(stream);
            if (contents.Length > MaxFileSize)
                throw new ArgumentException(
                    $"File size {contents.Length.BytesDisplay()} exceeds the maximum size of {MaxFileSize.BytesDisplay()}.");

            lock (_lock)
            {
                _storage[path] = Tuple.Create(new FileSpec(path, Path.GetFileName(path), DateTime.UtcNow, DateTime.UtcNow, default(DateTime?), contents.LongLength) as IFileSpec, contents);

                if (_storage.Count > MaxFiles)
                    _storage.Remove(_storage.OrderByDescending(kvp => kvp.Value.Item1.CreationDate).First().Key);
            }

            return Task.FromResult(true);
        }

        public Task<bool> RenameFileAsync(string path, string newpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (String.IsNullOrWhiteSpace(newpath))
                throw new ArgumentNullException(nameof(newpath));

            lock (_lock)
            {
                if (!_storage.ContainsKey(path))
                    return Task.FromResult(false);

                _storage[newpath] = _storage[path];
                _storage[newpath].Item1.ModifyPath(newpath);
                _storage.Remove(path);
            }

            return Task.FromResult(true);
        }

        public Task<bool> CopyFileAsync(string path, string targetpath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (String.IsNullOrWhiteSpace(targetpath))
                throw new ArgumentNullException(nameof(targetpath));

            lock (_lock)
            {
                if (!_storage.ContainsKey(path))
                    return Task.FromResult(false);

                _storage[targetpath] = _storage[path];
                _storage[targetpath].Item1.ModifyPath(targetpath);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteFileAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            lock (_lock)
            {
                if (!_storage.ContainsKey(path))
                    return Task.FromResult(false);

                _storage.Remove(path);
            }

            return Task.FromResult(true);
        }

        public Task<IEnumerable<IFileSpec>> GetFileListAsync(string searchPattern = null, int? limit = null, int? skip = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (limit.HasValue && limit.Value <= 0)
                return Task.FromResult<IEnumerable<IFileSpec>>(new List<IFileSpec>());

            if (searchPattern == null)
                searchPattern = "*";

            var regex = new Regex("^" + Regex.Escape(searchPattern).Replace("\\*", ".*?") + "$");
            lock (_lock)
                return Task.FromResult<IEnumerable<IFileSpec>>(_storage.Keys.Where(k => regex.IsMatch(k)).Select(k => _storage[k].Item1).Skip(skip ?? 0).Take(limit ?? Int32.MaxValue).ToList());
        }

        public void Dispose()
        {
            _storage?.Clear();
        }
    }
}