using System;
using System.Collections.Generic;
using Borg.Infra.Hierarchy;

namespace Borg.Framework.Services.Navigation.Contracts
{
    /// <summary>
    /// <see>
    ///     <cref>INavigationElements</cref>
    /// </see>
    ///     is a contract for an hierachical collection of naviagation items.
    /// </summary>
    public interface INavigationElements<in TKey> : IHierarchicalEnumerable
        where TKey : IEquatable<TKey>
    {
    }

    public interface INavigationElementContainer<in TKey, out TElement>
        where TKey : struct, IEquatable<TKey> where TElement : INavigationElement<TKey>
    {
        IReadOnlyCollection<TElement> Data { get; }
    }
}