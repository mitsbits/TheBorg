using System;
using Borg.Framework.Backoffice.Identity.Models;
using Borg.Framework.Backoffice.Pages.Data;
using Borg.Framework.System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Borg.Framework.Backoffice.Identity.Data
{
    public class IdentityDbContext : IdentityDbContext<BorgUser>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }

    public class IdentityDbTransactionRepository<T> : BaseReadWriteRepository<T, IdentityDbContext>, ICRUDRespoditory<T> where T : class
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public override IdentityDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<IdentityDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException(
                        $@"No ambient DbContext of type {typeof(PagesDbContext).Name} found.
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

        public IdentityDbTransactionRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException(nameof(ambientDbContextLocator));
            _ambientDbContextLocator = ambientDbContextLocator;
        }
    }

    public class IdentityDbQueryRepository<T> : BaseQueryRespository<T, IdentityDbContext> where T : class
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public override IdentityDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<IdentityDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException(
                        $@"No ambient DbContext of type {typeof(PagesDbContext).Name} found.
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

        public IdentityDbQueryRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException(nameof(ambientDbContextLocator));
            _ambientDbContextLocator = ambientDbContextLocator;
        }
    }

    public class IdentityDbContextFactory : IDbContextFactory<IdentityDbContext>
    {
        private readonly BorgSettings _settings;
        private readonly string _cs = string.Empty;

        public IdentityDbContextFactory(BorgSettings settings)
        {
            _settings = settings;
        }

        public IdentityDbContextFactory()
        {
            //_cs = "Server=.\\x2014;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
            _cs = "Server=.\\SQL2016;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true;";
        }

        public IdentityDbContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext>();
            if (_settings != null)
            {
                var ops =
                    builder.UseSqlServer(_settings.Backoffice.Application.Data.Relational.ConsectionStringIndex["borg"])
                        .Options;
                var context = new IdentityDbContext(ops);
                return context;
            }
            else
            {
                var ops =
                    builder.UseSqlServer(_cs, optionsBuilder => optionsBuilder.MigrationsAssembly("Framework.Backoffice"))
                        .Options;
                var context = new IdentityDbContext(ops);
                return context;
            }
        }
    }
}