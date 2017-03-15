using Borg.Infra.Relational;
using Borg.Infra.Storage.Assets;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Borg.Framework.System;

namespace Borg.Framework.Media
{
    [BorgModule]
    public class AssetsMetadataStorage : IAssetMetadataStorage<int>, IDisposable
    {
        private readonly ILogger Loger;
        private readonly AssetsDbContext _dbContext;
        private readonly ICRUDRespoditory<AssetSpec> _repo;
        private readonly ICRUDRespoditory<FileSpec> _filerepo;

        public AssetsMetadataStorage(ILoggerFactory loggerFactory, AssetsDbContext dbContext)
        {
            Loger = loggerFactory.CreateLogger(GetType());
            _dbContext = dbContext;
            _repo = new AssetsDbRepository<AssetSpec>(_dbContext);
            _filerepo = new AssetsDbRepository<FileSpec>(_dbContext);
        }

        public async Task Activate(int id)
        {
            var asset = await _repo.GetAsync(x => x.Id == id);
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            asset.Activate();
            await _repo.UpdateAsync(asset);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Add(IAssetSpec<int> spec)
        {
            var f = spec.CurrentFile.FileSpec;
            var file = new FileSpec() { Name = f.Name, FullPath = f.FullPath, CreationDate = f.CreationDate, LastRead = f.LastRead, LastWrite = f.LastWrite, MimeType = f.MimeType, SizeInBytes = f.SizeInBytes };
            var version = new VersionSpec() { FileSpec = file, Version = 1 };
            var asset = new AssetSpec() { Name = spec.Name, Id = spec.Id };
            if (spec.State == AssetState.Active) asset.Activate();
            if (spec.State == AssetState.Suspended) asset.Deactivate();
            asset.Versions.Add(version);

            await _repo.CreateAsync(asset);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddVersion(int id, IVersionSpec spec)
        {
            var asset = await _repo.GetAsync(x => x.Id == id, assetSpec => assetSpec.Versions);
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            var file = new FileSpec() { Name = spec.FileSpec.Name, FullPath = spec.FileSpec.FullPath, MimeType = spec.FileSpec.MimeType, CreationDate = spec.FileSpec.CreationDate, LastWrite = spec.FileSpec.LastWrite, LastRead = spec.FileSpec.LastRead, SizeInBytes = spec.FileSpec.SizeInBytes };
            var version = new VersionSpec() { FileSpec = file, Version = spec.Version };
            asset.Versions.Add(version);
            await _repo.UpdateAsync(asset);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Deactivate(int id)
        {
            var asset = await _repo.GetAsync(x => x.Id == id);
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            asset.Deactivate();
            await _repo.UpdateAsync(asset);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IAssetSpec<int>> Get(int id)
        {
            var asset = await _repo.GetAsync(x => x.Id == id, assetSpec => assetSpec.Versions);
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            var versions = asset.Versions.Select(x => x.Id).ToArray();
            var files = await _filerepo.FindAsync(x => versions.Contains(x.VersionId),
                new[] { new OrderByInfo<FileSpec>() { Ascending = false, Property = v => v.Id }, });
            foreach (var assetVersion in asset.Versions)
            {
                assetVersion.FileSpec = files.Records.Single(x => x.VersionId == assetVersion.Id);
            }
            return asset;
        }

        public async Task Remove(int id)
        {
            await _repo.DeleteAsync(x => x.Id == id);
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}