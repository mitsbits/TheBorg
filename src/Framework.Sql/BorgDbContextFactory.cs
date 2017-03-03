using System;
using Borg.Framework.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Borg.Framework.Sql
{
    public delegate void OnMigrationConfiguringHandler(out string connectionString, out Action<SqlServerDbContextOptionsBuilder>  migrationAction) ;
    public abstract class BorgDbContextFactory<TDbContext, TSettings> : IDbContextFactory<TDbContext> where TDbContext : DbContext where TSettings : BorgSettings
    {
        private readonly TSettings _settings;

        protected BorgDbContextFactory(TSettings settings)
        {
            _settings = settings;
        }

        protected BorgDbContextFactory()
        {
   
        }

        protected  OnMigrationConfiguringHandler OnMigrationConfiguring { get; set; } = null;

        protected virtual string ConnectionStringKey { get; set; } = "borg";
        public TDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();
            DbContextOptions<TDbContext> ops;
            if (_settings != null)
            {
                 ops = builder.UseSqlServer(_settings.Backoffice.Application.Data.Relational.ConnectionStringIndex[ConnectionStringKey]).Options;     
            }
            else
            {
                string connectionString = string.Empty;
                Action<SqlServerDbContextOptionsBuilder> migrationAction = null;
                OnMigrationConfiguring?.Invoke(out connectionString, out migrationAction);
                ops = builder.UseSqlServer(connectionString, migrationAction).Options;
            }
            var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), ops);
            return context;
        }
    }
}