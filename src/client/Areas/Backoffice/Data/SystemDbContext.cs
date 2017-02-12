using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Borg.Infra.EF6;
using Borg.Infra.Relational;
using Framework.System.Domain;
using Mehdime.Entity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Borg.Client.Areas.Backoffice.Data
{
    public class SystemDbContext : DiscoveryDbContext
    {
        public SystemDbContext(DiscoveryDbContextSpec spec) : base(spec)
        {
            
        }

        public Microsoft.EntityFrameworkCore.DbSet<Page> Pages { get; set; }

        //public SystemDbContext():base()  { }
        //public SystemDbContext(DbContextOptions<SystemDbContext> options) : base(options) { }
        //public SystemDbContext(string connectionString) : base(connectionString)
        //{
        //}

        protected override void OnModelCreating(DbModelBuilder builder)
        {

            builder.Configurations.Add(new PageConfig());

        }


        private class PageConfig : EntityTypeConfiguration<Page>
        {
            public PageConfig()
            {
                ToTable("Pages").HasKey(x => new { x.Id });

            }
        }
    }

    //public class SystemDbContextFactory : IDbContextFactory<SystemDbContext>
    //{
    //    public SystemDbContext Create(DbContextFactoryOptions options)
    //    {
    //        return new SystemDbContext("Server=.\\x2014;Database=borg;Trusted_Connection=True;MultipleActiveResultSets=true");
    //    }
    //}



    public class SystemEntityRepository<T> : ContextScopedReadWriteRepository<T, SystemDbContext>, ICRUDRespoditory<T> where T : class
    {
        public SystemEntityRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }
    }
}