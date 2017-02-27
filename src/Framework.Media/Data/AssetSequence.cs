using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Borg.Framework.Media
{
    public class AssetSequence : IUniqueKeyProvider<int>
    {
        private readonly AssetsDbContext _dbContext;

        public AssetSequence(AssetsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Pop()
        {
            var val = await _dbContext.AssetSequence.AsNoTracking()
                .FromSql(
                    "SELECT NextId = NEXT VALUE FOR AssetsSequence ").ToListAsync();
            return val.First().NextId;
        }
    }
}