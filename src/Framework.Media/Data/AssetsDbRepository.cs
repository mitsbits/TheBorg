using System;
using Borg.Infra.EFCore;
using Borg.Infra.Relational;

namespace Borg.Framework.Media
{
    public class AssetsDbRepository<T> : BaseReadWriteRepository<T, AssetsDbContext>, ICRUDRespoditory<T> where T : class
    {
        public override AssetsDbContext DbContext { get; }

        public AssetsDbRepository(AssetsDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            DbContext = dbContext;
        }
    }
}