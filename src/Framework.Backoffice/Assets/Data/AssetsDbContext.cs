using Borg.Framework.Backoffice.Assets.Data;
using Borg.Framework.System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Borg.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Framework.Backoffice.Assets.Data
{
    public class AssetsDbContext : DbContext
    {
        public AssetsDbContext() : base()
        {
        }

        public AssetsDbContext(DbContextOptions<AssetsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ForSqlServerHasSequence<int>("AssetsSequence").StartsAt(1).IncrementsBy(1);
            modelBuilder.Entity<AssetSpec>().Property(x => x.Id).HasDefaultValueSql("NEXT VALUE FOR AssetsSequence");
        }

        public DbSet<FileSpec> Files { get; set; }
        public DbSet<VersionSpec> Versions { get; set; }
        public DbSet<AssetSpec> Assets { get; set; }
        internal DbSet<AssetSequenceValue> AssetSequence { get; set; }

        internal class AssetSequenceValue
        {
            protected AssetSequenceValue()
            {
            }

            [Key]
            public int NextId { get; protected set; }
        }
    }

    public class AssetsDbRepository<T> : BaseReadWriteRepository<T, AssetsDbContext>, ICRUDRespoditory<T> where T : class
    {
        public override AssetsDbContext DbContext { get; }

        public AssetsDbRepository(AssetsDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            DbContext = dbContext;
        }
    }

    public class AssetsDbContextFactory : IDbContextFactory<AssetsDbContext>
    {
        private readonly BorgSettings _settings;
        private readonly string _cs = string.Empty;

        public AssetsDbContextFactory(BorgSettings settings)
        {
            _settings = settings;
        }

        public AssetsDbContextFactory()
        {
            _cs = "Server=.\\x2014;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
        }

        public AssetsDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<AssetsDbContext>();
            if (_settings != null)
            {
                var ops =
                    builder.UseSqlServer(_settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["borg"])
                        .Options;
                var context = new AssetsDbContext(ops);
                return context;
            }
            else
            {
                var ops =
                   builder.UseSqlServer(_cs)
                       .Options;
                var context = new AssetsDbContext(ops);
                return context;
            }
        }
    }

    public class FileSpec : IFileSpec<int>
    {
        public FileSpec()
        {
        }

        public int VersionId { get; set; }
        public virtual VersionSpec Version { get; set; }

        public string FullPath { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastWrite { get; set; }
        public DateTime? LastRead { get; set; }
        public long SizeInBytes { get; set; }
        public string MimeType { get; set; }

        public void ModifyPath(string newPath)
        {
            if (FullPath == newPath) return;
            FullPath = newPath;
        }

        [Key]
        public int Id { get; protected set; }
    }

    public class VersionSpec : IVersionSpec
    {
        public VersionSpec()
        {
        }

        [Key]
        public int Id { get; set; }

        public int AssetId { get; set; }
        public virtual AssetSpec Asset { get; set; }
        public int Version { get; set; }
        public FileSpec FileSpec { get; set; }
        IFileSpec IVersionSpec.FileSpec => FileSpec;
    }

    public class AssetSpec : IAssetSpec<int>
    {
        public AssetSpec()
        {
            Versions = new HashSet<VersionSpec>();
        }

        public AssetState State { get; protected set; }

        public virtual VersionSpec CurrentFile
        {
            get { return Versions.OrderByDescending(x => x.Version).First(); }
        }

        [MaxLength(512)]
        public string Name { get; set; }

        [Key]
        public int Id { get; set; }

        public virtual ICollection<VersionSpec> Versions { get; protected set; }

        IEnumerable<IVersionSpec> IAssetSpec.Versions => Versions;

        IVersionSpec IAssetSpec.CurrentFile
        {
            get { return Versions.OrderByDescending(x => x.Version).First(); }
        }

        public void Activate()
        {
            if (State == AssetState.Active) return;
            State = AssetState.Active;
        }

        public void Deactivate()
        {
            if (State == AssetState.Suspended) return;
            State = AssetState.Suspended;
        }
    }

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
                new[] {new OrderByInfo<FileSpec>() {Ascending = false, Property = v => v.Id},});
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

    public class MediaService : BaseAssetService<int>, IMediaService, IDisposable
    {
        private readonly AssetsDbContext _dbContext;
        private readonly ICRUDRespoditory<AssetSpec> _repo;
        private readonly ICRUDRespoditory<FileSpec> _fileRepo;

        public MediaService(ILoggerFactory loggerFactory, IFileStorage storage, IUniqueKeyProvider<int> keyProvider, IConflictingNamesResolver namesResolver, IAssetMetadataStorage<int> db, IFolderScopeFactory<int> folderScope, AssetsDbContext dbContext) : base(loggerFactory, storage, keyProvider, namesResolver, db, folderScope)
        {
            _dbContext = dbContext;
            _repo = new AssetsDbRepository<AssetSpec>(_dbContext);
            _fileRepo = new AssetsDbRepository<FileSpec>(_dbContext);
        }

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

        #endregion IDisposable Support
    }

    public interface IMediaService : IAssetService<int>, IDisposable
    {
        Task<IPagedResult<AssetSpec>> Assets(Expression<Func<AssetSpec, bool>> predicate, int page, int size, IEnumerable<OrderByInfo<AssetSpec>> orderBy, params Expression<Func<AssetSpec, dynamic>>[] paths);

        Task AssetChangeName(int id, string name);

        Task AssetChangeState(int id, AssetState state);
    }

    public class AssetSequence : IUniqueKeyProvider<int>
    {
        private readonly AssetsDbContext _dbContext;

        public AssetSequence(AssetsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Pop()
        {
            var val = await _dbContext.AssetSequence.AsNoTracking()
                    .FromSql(
                        "SELECT NextId = NEXT VALUE FOR AssetsSequence ").ToListAsync();
            return val.First().NextId;
        }
    }
}

namespace Borg
{
    internal static class IMediaServiceExtensions
    {
        public static async Task<Framework.Backoffice.Assets.Data.AssetSpec> Get(this IMediaService service, int id)
        {
            var hits = await service.Assets(x => x.Id == id, 1, 1,
                new[]
                {
                    new OrderByInfo<Framework.Backoffice.Assets.Data.AssetSpec>()
                    {
                        Ascending = true,
                        Property = x => x.Id
                    },
                }, spec => spec.Versions);
            return hits.Records.FirstOrDefault();
        }
    }
}