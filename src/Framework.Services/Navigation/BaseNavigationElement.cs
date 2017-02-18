using Borg.Framework.Services.Navigation.Contracts;
using Borg.Infra.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Borg.Framework.Services.Navigation
{
    public class BaseNavigationElement<TKey> : INavigationElement<TKey>, IHasParent where TKey : IEquatable<TKey>
    {
        private const string PathSeperator = @".";
        private readonly IEnumerable<INavigationElement<TKey>> _data;
        private readonly BaseNavigationElement<TKey> _proxy;
        private readonly int _depth;

        public BaseNavigationElement(IEnumerable<INavigationElement<TKey>> data, TKey id)
        {
            _data = data?.ToArray();
            if (Data.All(x => x.Id.Equals(id))) throw new NullReferenceException("proxy");
            _proxy = Data.FirstOrDefault(x => x.Id.Equals(id)) as BaseNavigationElement<TKey>;
            _depth = CalculateDepth();
        }

        private int CalculateDepth()
        {
            var d = 1;
            var id = ParentId;
            while (id.Equals(default(TKey)))
            {
                d++;
                id = Data.Any(x => x.Id.Equals(id)) ? Data.First(x => x.Id.Equals(id)).ParentId : default(TKey);
            }
            return d;
        }

        public NavigationElementRole Role => Proxy.Role;

        public string AnchorTarget => Proxy.AnchorTarget;

        public string Display => Proxy.Display;

        public string Href => Proxy.Href;

        public int Depth => _depth;

        public Func<INavigationElement<TKey>, string> IconCssClass => Proxy.IconCssClass;

        public bool HasChildren
        {
            get { return Data.Any(x => x.ParentId.Equals(Proxy.Id)); }
        }

        public object Item => this;

        public string Type => GetType().FullName;

        protected virtual IHierarchicalEnumerable GetChildren()
        {
            var result = new BaseNavigationElements<BaseNavigationElement<TKey>, TKey>(
                Data.Where(x => x.ParentId.Equals(Proxy.Id))
                .OrderBy(x => x.Weight)
                .Select(x => new BaseNavigationElement<TKey>(Data, x.Id))
                .ToList());
            return result;
        }

        public IHierarchicalEnumerable Children => GetChildren();

        protected virtual IHierarchyData GetParent()
        {
            return Data.Any(x => x.Id.Equals(Proxy.ParentId)) ? new BaseNavigationElement<TKey>(Data, Proxy.ParentId) : null;
        }

        public IHierarchyData Parent => GetParent();

        public double Weight => Proxy.Weight;

        public TKey Id => Proxy.Id;

        public TKey ParentId => Proxy.ParentId;

        protected IEnumerable<INavigationElement<TKey>> Data => _data;

        protected BaseNavigationElement<TKey> Proxy => _proxy;
    }
}