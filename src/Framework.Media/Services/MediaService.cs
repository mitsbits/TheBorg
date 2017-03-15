using Borg.Infra.CQRS;
using Borg.Infra.Relational;
using Borg.Infra.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Borg.Framework.System;
using Borg.Infra.Storage.Assets;

namespace Borg.Framework.Media
{
    [BorgModule]
    public class MediaService : BaseAssetService<int>, IMediaService, IDisposable
    {
        private readonly AssetsDbContext _dbContext;
        private readonly ICRUDRespoditory<AssetSpec> _repo;
        private readonly ICRUDRespoditory<FileSpec> _fileRepo;

        public MediaService(ILoggerFactory loggerFactory, IFileStorage storage, IUniqueKeyProvider<int> keyProvider, IConflictingNamesResolver namesResolver, IAssetMetadataStorage<int> db, IFolderScopeFactory<int> folderScope, AssetsDbContext dbContext, IEventBus events) : base(loggerFactory, storage, keyProvider, namesResolver, db, folderScope, events)
        {
            _dbContext = dbContext;
            _repo = new AssetsDbRepository<AssetSpec>(_dbContext);
            _fileRepo = new AssetsDbRepository<FileSpec>(_dbContext);
        }

        #region IMediaService

        public async Task<IPagedResult<AssetSpec>> Assets(Expression<Func<AssetSpec, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<AssetSpec>> orderBy, params Expression<Func<AssetSpec, dynamic>>[] paths)
        {
            var data = await _repo.FindAsync(predicate, page, size, orderBy, paths);
            var vIds = data.Records.SelectMany(r => r.Versions).Select(v => v.Id).Distinct().ToList();
            var vers = await _fileRepo.FindAsync(x => vIds.Contains(x.VersionId), 1, 1000,
                new[] { new OrderByInfo<FileSpec>() { Ascending = true, Property = p => p.Id } });
            var files = vers.Records;
            foreach (var dataRecord in data.Records)
            {
                foreach (var dataRecordVersion in dataRecord.Versions)
                {
                    var f = files.Single(x => x.VersionId == dataRecordVersion.Id);
                    dataRecordVersion.FileSpec = f;
                }
            }
            return data;
        }

        public async Task AssetChangeName(int id, string name)
        {
            var asset = await _repo.GetAsync(x => x.Id == id);
            asset.Name = name;
            await _repo.UpdateAsync(asset);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AssetChangeState(int id, AssetState state)
        {
            var asset = await _repo.GetAsync(x => x.Id == id);
            if (state == AssetState.Active) asset.Activate();
            if (state == AssetState.Suspended) asset.Deactivate();
            await _repo.UpdateAsync(asset);
            await _dbContext.SaveChangesAsync();
        }

        #endregion IMediaService

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MediaService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}