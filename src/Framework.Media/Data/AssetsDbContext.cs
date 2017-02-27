using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Borg.Framework.Media
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
}

