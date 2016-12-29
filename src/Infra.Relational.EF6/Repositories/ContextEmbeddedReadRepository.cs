using System;
using System.Data.Entity;

namespace Borg.Infra.Relational.EF6
{
    public abstract class ContextEmbeddedReadRepository<T, TDbContext> :
    BaseReadRepository<T, TDbContext>
    where TDbContext : DbContext
    where T : class
    {
        private readonly TDbContext _dbContext;

        protected ContextEmbeddedReadRepository(TDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            _dbContext = dbContext;
        }

        public override TDbContext DbContext => _dbContext;
    }
}