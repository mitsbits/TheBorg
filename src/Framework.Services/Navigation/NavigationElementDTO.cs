using System;

namespace Borg.Framework.Services.Navigation
{
    public class NavigationElementDto<TKey>  where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public TKey ParentId { get; set; }

        public NavigationElementRole Role { get; set; }

        public string AnchorTarget { get; set; }

        public string Display { get; set; }

        public string Href { get; set; }


        public double Weight { get; set; }

        public Func<NavigationElementDto<TKey>, string> IconCssClass { get; set; }
    }
}