using System;

namespace Borg.Infra.Storage
{
    public interface IFolderScopeFactory<in TKey> where TKey : IEquatable<TKey>
    {
        Func<TKey, string> ScopeFactory { get; }
    }

    public class DefaultFolderIntegerScopeFactory : IFolderScopeFactory<int>
    {
        public DefaultFolderIntegerScopeFactory(Func<int, string> scopeFactory = null)
        {
            ScopeFactory = (scopeFactory == null)
                ? ScopeFactory = (i) => (i.RoundOff(100) + 100).ToString()
                : scopeFactory;
        }
        public Func<int, string> ScopeFactory { get; }
    }
}