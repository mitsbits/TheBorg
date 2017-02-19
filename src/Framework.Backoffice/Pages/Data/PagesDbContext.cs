﻿using System;
using Framework.System.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;

namespace Borg.Framework.Backoffice.Pages.Data
{
    public class PagesDbContext : DbContext
    {
        public DbSet<SimplePage> Simples { get; set; }
        public PagesDbContext() : base()
        {
        }
        public PagesDbContext(DbContextOptions<PagesDbContext> options)
            : base(options)
        {
        }
    }

    public class PagesDbRepository<T> : BaseReadWriteRepository<T, PagesDbContext>, ICRUDRespoditory<T> where T : class
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public override PagesDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<PagesDbContext>();

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

        public PagesDbRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null) throw new ArgumentNullException(nameof(ambientDbContextLocator));
            _ambientDbContextLocator = ambientDbContextLocator;
        }
    }

    [Table("SimplePages")]
    public class SimplePage : Component
    {
        [Key]
        public override int Id { get; protected set; }

        [StringLength(256)]
        public string Title { get;  set; }

        [StringLength(256)]
        public string Path { get;  set; }

        public string Body { get; protected set; }
    }
}