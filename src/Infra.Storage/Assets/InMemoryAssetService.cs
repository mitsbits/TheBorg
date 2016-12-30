using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Borg.Infra.Storage
{
    public class InMemoryAssetService<TKey> : IAssetService<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IFileStorage _storage;
        private readonly IUniqueKeyProvider<TKey> _keyProvider;
        private readonly IConflictingNamesResolver _namesResolver;

        private readonly Dictionary<TKey, Tuple<AssetSpec<TKey>, List<IVersionSpec>>> _db =
            new Dictionary<TKey, Tuple<AssetSpec<TKey>, List<IVersionSpec>>>();

        private readonly object _lock = new object();

        public InMemoryAssetService(IFileStorage storage, IUniqueKeyProvider<TKey> keyProvider, IConflictingNamesResolver namesResolver)
        {
            _storage = storage;
            _keyProvider = keyProvider;
            _namesResolver = namesResolver;
        }

        public async Task<IAssetSpec<TKey>> Create(string name, byte[] content, string fileName, AssetState state, string contentType = "")
        {
            try
            {
                var exists = await _storage.ExistsAsync(fileName);
                if (exists) fileName = await _namesResolver.Resolve(fileName);
                await _storage.SaveFileAsync(fileName, new MemoryStream(content));
                var filespec = await _storage.GetFileInfoAsync(fileName);
                var versionSpec = new VersionSpec(1, filespec);
                var spec = new AssetSpec<TKey>(await _keyProvider.Pop(), AssetState.Active, versionSpec, fileName);
                lock (_lock)
                {
                    _db.Add(spec.Id, Tuple.Create(spec, new List<IVersionSpec>() { versionSpec }));
                }
                return spec;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task Suspend(TKey id)
        {
            lock (_lock)
            {
                var hit = _db[id].Item1;
                hit?.Deactivate();
            }
            return Task.CompletedTask;
        }

        public Task Acivate(TKey id)
        {
            lock (_lock)
            {
                var hit = _db[id].Item1;
                hit?.Activate();
            }
            return Task.CompletedTask;
        }

        public Task Delete(TKey id)
        {
            lock (_lock)
            {
                var hit = _db[id].Item1;
                if (hit == null) throw new AssetNotFoundException<TKey>(id);
                List<IVersionSpec> versions = _db[id].Item2;

                foreach (var versionSpec in versions)
                {
                    AsyncHelpers.RunSync(() => _storage.DeleteFileAsync(versionSpec.FileSpec.FullPath));
                }
                _db.Remove(id);
            }
            return Task.CompletedTask;
        }

        public async Task<IAssetSpec<TKey>> AddNewVersion(TKey id, byte[] content, string fileName, string contentType = "")
        {
            try
            {
                AssetSpec<TKey> spec;
                lock (_lock)
                {
                    spec = _db[id].Item1;
                }
                if (spec == null) throw new AssetNotFoundException<TKey>(id);
                List<IVersionSpec> versions = _db[id].Item2;
                var exists = await _storage.ExistsAsync(fileName);
                if (exists) fileName = await _namesResolver.Resolve(fileName);
                await _storage.SaveFileAsync(fileName, new MemoryStream(content));
                var filespec = await _storage.GetFileInfoAsync(fileName);
                var versionSpec = new VersionSpec(versions.Max(x => x.Version) + 1, filespec);
                versions.Add(versionSpec);
                spec = new AssetSpec<TKey>(spec.Id, spec.State, versionSpec, spec.Name);
                _db[id] = Tuple.Create(spec, versions);
                return spec;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}