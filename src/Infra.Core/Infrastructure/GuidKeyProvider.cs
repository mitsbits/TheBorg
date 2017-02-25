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

    public class NullKeyProvider<TKey> : IUniqueKeyProvider<TKey> where TKey : IEquatable<TKey>
    {
        public Task<TKey> Pop()
        {
            return Task.FromResult(default(TKey));
        }
    }
}