using System;
using System.Threading.Tasks;

namespace Borg
{
    public interface IUniqueKeyProvider<TKey> where TKey : IEquatable<TKey>
    {
        Task<TKey> Pop();
    }
}