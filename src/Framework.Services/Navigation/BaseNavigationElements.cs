using System;
using System.Collections;
using System.Collections.Generic;
using Borg.Framework.Services.Navigation.Contracts;
using Borg.Infra.Hierarchy;

namespace Borg.Framework.Services.Navigation
{
    public class BaseNavigationElements<TData, TKey> : INavigationElements<TKey> where TData : class, INavigationElement<TKey> where TKey : IEquatable<TKey>
    {
        protected readonly IEnumerable<TData> Descedants;

        public BaseNavigationElements(IEnumerable<TData> descedants)
        {
            Descedants = descedants;
        }

        public IEnumerator GetEnumerator()
        {
            if (Descedants == null) return new List<TData>().GetEnumerator();

            var enumurator = Descedants.GetEnumerator();
            return enumurator;
        }

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return enumeratedItem as IHierarchyData;
        }
    }
}