using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Borg.Infra.EFCore
{
    public abstract class DiscoveryDbContext : BaseSqlDbContext
    {
        protected DiscoveryDbContext(string connectionString) : base(connectionString)
        {
        }
    }

    public abstract class BaseSqlDbContext : DbContext
    {
        protected string ConnectionString { get; }

        protected BaseSqlDbContext() : base()
        {
        }

        protected BaseSqlDbContext(DbContextOptions options) : base(options)
        {
        }

        protected BaseSqlDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(ConnectionString, SqlServerOptionsAction);

        protected virtual void SqlServerOptionsAction(SqlServerDbContextOptionsBuilder sqlServerDbContextOptionsBuilder)
        {
        }
    }
}