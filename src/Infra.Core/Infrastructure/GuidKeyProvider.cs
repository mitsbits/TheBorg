using System;
using System.Threading.Tasks;

namespace Borg
{
    public class GuidKeyProvider : IUniqueKeyProvider<Guid>
    {
        public Task<Guid> Pop()
        {
            return Task.FromResult(Guid.NewGuid());
        }
    }
}