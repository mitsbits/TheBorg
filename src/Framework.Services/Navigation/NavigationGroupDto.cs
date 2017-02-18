using Borg.Framework.Services.Navigation;
using System;

namespace Borg.Framework.Services.Navigation
{
    public struct NavigationGroupDto<TKey> where TKey : IEquatable<TKey>
    {
        public NavigationElementDto<TKey> Header { get; set; }

        public  BaseNavigationElements<BaseNavigationElement<TKey>, TKey> Elements { get; set; }

    }
}