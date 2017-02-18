using System;
using System.Collections.Generic;

namespace Borg.Framework.Services.Navigation.Contracts
{
    public interface IMenuProvider<out TCollection, TKey> where TCollection : INavigationElements<TKey> where TKey : IEquatable<TKey>
    {
        ICollection<NavigationElementDto<TKey>> Raw { get; }
        TCollection Menu { get; }
    }
}