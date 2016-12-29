using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Borg.Infra.Relational.EF6
{
    public abstract class ContextEmbeddedReadWriteRepositoryWithCommit<T, TDbContext> :
        BaseReadRepository<T, TDbContext>, IRepositoryThatCommits
        where TDbContext : DbContext
        where T : class
    {
        private readonly TDbContext _dbContext;

        protected ContextEmbeddedReadWriteRepositoryWithCommit(TDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            _dbContext = dbContext;
        }

        public override TDbContext DbContext => _dbContext;

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }
}