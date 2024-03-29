﻿
using System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;
using Framework.System.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Borg.Client.Areas.Backoffice.Data
{
    public class SystemDbContext : DiscoveryDbContext
    {

        public SystemDbContext(DbContextOptions options) : base(options)
        {
        }

        //public SystemDbContext() : base()
        //{
        //}

        public Microsoft.EntityFrameworkCore.DbSet<Page> Pages { get; set; }

        //public SystemDbContext():base()  { }
        //public SystemDbContext(DbContextOptions<SystemDbContext> options) : base(options) { }
        //public SystemDbContext(string connectionString) : base(connectionString)
        //{
        //}



        //private class PageConfig : EntityTypeConfiguration<Page>
        //{
        //    public PageConfig()
        //    {
        //        ToTable("Pages").HasKey(x => new { x.Id });

        //    }
        //}
    }

    //public class SystemDbContextFactory : IDbContextFactory<SystemDbContext>
    //{
    //    public SystemDbContext Create(DbContextFactoryOptions options)
    //    {
    //        return new SystemDbContext("Server=.\\x2014;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true");
    //    }
    //}



    public class SystemEntityRepository<T> : BaseReadWriteRepository<T, SystemDbContext>, ICRUDRespoditory<T> where T : class
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public override SystemDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<SystemDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException(
                        $@"No ambient DbContext of type {typeof(SystemDbContext).Name} found.
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

        public SystemEntityRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException(nameof(ambientDbContextLocator));
            _ambientDbContextLocator = ambientDbContextLocator;
        }
    }
}