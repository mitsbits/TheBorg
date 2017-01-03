using Mehdime.Entity;
using System;
using System.Data.Entity;

namespace Borg.Infra.EF6
{
    public abstract class AutoTransientReadWriteRepository<T, TDbContext> :
      BaseReadRepository<T, TDbContext>
      where TDbContext : DbContext
      where T : class
    {
        private readonly IDbContextFactory _contextFactory;
        private TDbContext _dbContext;

        protected AutoTransientReadWriteRepository(IDbContextFactory contextFactory)
        {
            if (contextFactory == null) throw new ArgumentNullException(nameof(contextFactory));
            _contextFactory = contextFactory;
            PreProcess = () =>
            {
                var db = _contextFactory.CreateDbContext<TDbContext>();
                if (db == null) throw new ArgumentNullException(nameof(db));
                _dbContext = db;
            };
            PostProcessQuery = () =>
            {
                _dbContext?.Dispose();
            };
            PostProcessUpdateAsync = async () =>
            {
                await _dbContext?.SaveChangesAsync();
                _dbContext?.Dispose();
            };
        }

        public override TDbContext DbContext => _dbContext;
    }
}