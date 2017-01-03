using System;
using System.Data.Entity;

namespace Borg.Infra.EF6
{
    public abstract class ContextEmbeddedReadWriteRepository<T, TDbContext> :
        BaseReadRepository<T, TDbContext>
        where TDbContext : DbContext
        where T : class
    {
        private readonly TDbContext _dbContext;

        protected ContextEmbeddedReadWriteRepository(TDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            _dbContext = dbContext;
        }

        public override TDbContext DbContext => _dbContext;
    }
}