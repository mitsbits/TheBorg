using Borg.Framework.System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Borg.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

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

        public DbSet<FileSpec> Files { get; set; }
        public DbSet<VersionSpec> Versions { get; set; }
        public DbSet<AssetSpec> Assets { get; set; }
    }

    public class AssetsDbRepository<T> : BaseReadWriteRepository<T, AssetsDbContext>, ICRUDRespoditory<T> where T : class
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public override AssetsDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<AssetsDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException(
                        $@"No ambient DbContext of type {typeof(AssetsDbContext).Name} found.
                                        This means that this repository method has been called outside of the scope of a DbContextScope.
                                        A repository must only be accessed within the scope of a DbContextScope,
                                        which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts.
                                        This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction.
                                        To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction
                                        that your service method implements.
                                        Then access this repository within that scope.
                                        Refer to the comments in the IDbContextScope.cs file for more details.");

                return dbContext;
            }
        }

        public AssetsDbRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException(nameof(ambientDbContextLocator));
            _ambientDbContextLocator = ambientDbContextLocator;
        }
    }

    public class AssetsDbContextFactory : IDbContextFactory<AssetsDbContext>
    {
        private readonly BorgSettings _settings;

        public AssetsDbContextFactory(BorgSettings settings)
        {
            _settings = settings;
        }

        public AssetsDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<AssetsDbContext>();
            var ops =
                builder.UseSqlServer(_settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["borg"])
                    .Options;
            var context = new AssetsDbContext(ops);
            return context;
        }
    }

    public class FileSpec : IFileSpec<int>
    {
        protected FileSpec()
        {
        }

        public string FullPath { get; protected set; }
        public string Name { get; protected set; }
        public DateTime CreationDate { get; protected set; }
        public DateTime LastWrite { get; protected set; }
        public DateTime? LastRead { get; protected set; }
        public long SizeInBytes { get; protected set; }
        public string MimeType { get; protected set; }

        public void ModifyPath(string newPath)
        {
            if (FullPath == newPath) return;
            FullPath = newPath;
        }

        public int Id { get; protected set; }
    }

    public class VersionSpec : IVersionSpec
    {
        protected VersionSpec()
        {
        }

        public int Version { get; protected set; }
        public FileSpec FileSpec { get; protected set; }
        IFileSpec IVersionSpec.FileSpec => FileSpec;
    }

    public class AssetSpec : IAssetSpec<int>
    {
        protected AssetSpec()
        {
        }

        public AssetState State { get; protected set; }
        public VersionSpec CurrentFile { get; protected set; }
        IVersionSpec IAssetSpec.CurrentFile => CurrentFile;
        public string Name { get; protected set; }
        public int Id { get; protected set; }
    }
}