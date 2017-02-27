using Borg.Infra.Core.Storage.Assets;
using Borg.Infra.CQRS;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public abstract class BaseAssetService<TKey> : IAssetService<TKey> where TKey : IEquatable<TKey>
    {
        protected ILogger Logger { get; }
        protected readonly IFileStorage Storage;
        protected readonly IUniqueKeyProvider<TKey> KeyProvider;
        protected readonly IConflictingNamesResolver NamesResolver;
        protected readonly IAssetMetadataStorage<TKey> Db;
        protected readonly IFolderScopeFactory<TKey> FolderScope;
        protected readonly IEventBus Events;

        protected BaseAssetService(ILoggerFactory loggerFactory, IFileStorage storage, IUniqueKeyProvider<TKey> keyProvider, IConflictingNamesResolver namesResolver, IAssetMetadataStorage<TKey> db, IFolderScopeFactory<TKey> folderScope, IEventBus events)
        {
            Logger = loggerFactory.CreateLogger(GetType());
            Storage = storage;
            KeyProvider = keyProvider;
            NamesResolver = namesResolver;
            Db = db;
            FolderScope = folderScope;
            Events = events;
        }

        protected virtual IFileStorage ScopeStorage(TKey id)
        {
            return (FolderScope != null)
                ? new ScopedFileStorage(Storage, FolderScope.ScopeFactory.Invoke(id))
                : Storage;
        }

        public virtual async Task<IAssetSpec<TKey>> Create(string name, byte[] content, string fileName, AssetState state, string contentType = "")
        {
            var id = await KeyProvider.Pop();
            var storage = ScopeStorage(id);
            fileName = await ResoveNameIfExists(storage, fileName);
            var filespec = await StoreFile(content, fileName, storage);
            var versionSpec = new VersionSpec(1, filespec);
            var spec = new AssetSpec<TKey>(id, AssetState.Active, versionSpec, name);
            await Db.Add(spec);
            Logger.LogDebug("Created {@asset}", spec);
            await Events.Publish(new FileAddedToAssetEvent<TKey>(filespec, spec)).AnyContext();
            return spec;
        }

        public virtual async Task Suspend(TKey id)
        {
            await Db.Deactivate(id);
        }

        public virtual async Task Acivate(TKey id)
        {
            await Db.Activate(id);
        }

        public virtual async Task Delete(TKey id)
        {
            var hit = await Db.Get(id);
            if (hit == null) return;
            var versions = hit.Versions;

            //var storage = FolderScope(id); Do not use scoped storage, the full path is already prefixed

            foreach (var versionSpec in versions)
            {
                await Storage.DeleteFileAsync(versionSpec.FileSpec.FullPath);
            }
            await Db.Remove(id);
            Logger.LogDebug("Deleted {@asset}", hit);
        }

        public virtual async Task<IAssetSpec<TKey>> AddNewVersion(TKey id, byte[] content, string fileName, string contentType = "")
        {
            IAssetSpec<TKey> spec = await Db.Get(id);
            if (spec == null) throw new AssetNotFoundException<TKey>(id);
            var storage = ScopeStorage(id);
            fileName = await ResoveNameIfExists(storage, fileName);
            var filespec = await StoreFile(content, fileName, storage);
            var versionSpec = new VersionSpec(spec.Versions.Max(x => x.Version) + 1, filespec);
            await Db.AddVersion(id, versionSpec);
            Logger.LogDebug("Added {@version} to {@assr}", versionSpec, spec);
            await Events.Publish(new FileAddedToAssetEvent<TKey>(filespec, spec)).AnyContext();
            return spec;
        }

        protected virtual async Task<string> ResoveNameIfExists(IFileStorage storage, string fileName)
        {
            var exists = await storage.ExistsAsync(fileName);
            if (!exists) return fileName;
            var newFileName = await NamesResolver.Resolve(fileName);
            Logger.LogDebug("Naming {file} to {newfile}", fileName, newFileName);
            return newFileName;
        }

        protected virtual async Task<IFileSpec> StoreFile(byte[] content, string fileName, IFileStorage storage)
        {
            await storage.SaveFileAsync(fileName, new MemoryStream(content));
            var filespec = await storage.GetFileInfoAsync(fileName);
            Logger.LogDebug("Stored {@file}", filespec);
            return filespec;
        }
    }
}