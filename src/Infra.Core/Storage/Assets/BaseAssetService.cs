using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public abstract class BaseAssetService<TKey> : IAssetService<TKey> where TKey : IEquatable<TKey>
    {
        private ILogger Loger { get; }
        private readonly IFileStorage _storage;
        private readonly IUniqueKeyProvider<TKey> _keyProvider;
        private readonly IConflictingNamesResolver _namesResolver;
        private readonly IAssetMetadataStorage<TKey> _db;
        private readonly IFolderScopeFactory<TKey> _folderScope;

        protected BaseAssetService(ILoggerFactory loggerFactory, IFileStorage storage, IUniqueKeyProvider<TKey> keyProvider, IConflictingNamesResolver namesResolver, IAssetMetadataStorage<TKey> db, IFolderScopeFactory<TKey> folderScope)
        {
            Loger = loggerFactory.CreateLogger(GetType());
            _storage = storage;
            _keyProvider = keyProvider;
            _namesResolver = namesResolver;
            _db = db;
            _folderScope = folderScope;
        }

        private IFileStorage FolderScope(TKey id)
        {
            return (_folderScope != null)
                ? new ScopedFileStorage(_storage, _folderScope.ScopeFactory.Invoke(id))
                : _storage;
        }

        public async Task<IAssetSpec<TKey>> Create(string name, byte[] content, string fileName, AssetState state, string contentType = "")
        {
            var id = await _keyProvider.Pop();
            var storage = FolderScope(id);
            var exists = await storage.ExistsAsync(fileName);
            if (exists) fileName = await _namesResolver.Resolve(fileName);
            await storage.SaveFileAsync(fileName, new MemoryStream(content));
            var filespec = await storage.GetFileInfoAsync(fileName);
            var versionSpec = new VersionSpec(1, filespec);
            var spec = new AssetSpec<TKey>(id, AssetState.Active, versionSpec, name);
            await _db.Add(spec);
            return spec;
        }

        public async Task Suspend(TKey id)
        {
            await _db.Deactivate(id);
        }

        public async Task Acivate(TKey id)
        {
            await _db.Activate(id);
        }

        public async Task Delete(TKey id)
        {
            var hit = await _db.Get(id);
            if (hit == null) throw new AssetNotFoundException<TKey>(id);
            var versions = hit.Versions;

            //var storage = FolderScope(id); Do not use scoped storage, the full path is already prefixed

            foreach (var versionSpec in versions)
            {
                await _storage.DeleteFileAsync(versionSpec.FileSpec.FullPath);
            }
            await _db.Remove(id);
        }

        public async Task<IAssetSpec<TKey>> AddNewVersion(TKey id, byte[] content, string fileName, string contentType = "")
        {
            IAssetSpec<TKey> spec = await _db.Get(id);
            if (spec == null) throw new AssetNotFoundException<TKey>(id);
            var storage = FolderScope(id);
            var exists = await storage.ExistsAsync(fileName);
            if (exists) fileName = await _namesResolver.Resolve(fileName);
            await storage.SaveFileAsync(fileName, new MemoryStream(content));
            var filespec = await storage.GetFileInfoAsync(fileName);
            var versionSpec = new VersionSpec(spec.Versions.Max(x => x.Version) + 1, filespec);

            await _db.AddVersion(id, versionSpec);
            return spec;

        }
    }
}