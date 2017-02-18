using System;
using Borg.Infra.DTO;
using Borg.Infra.Hierarchy;

namespace Borg.Framework.Services.Navigation.Contracts
{
    public interface INavigationElement<TKey> : IHierarchyData, IWeighted, IHasParent<TKey> where TKey : IEquatable<TKey>
    {
        NavigationElementRole Role { get; }

        /// <summary>
        /// Gets or sets the anchor target.
        /// </summary>
        /// <value>
        /// The anchor target.
        /// </value>
        string AnchorTarget { get; }

        /// <summary>
        /// Gets or sets the display content of the element.
        /// </summary>
        /// <value>
        /// The display content.
        /// </value>
        string Display { get; }

        /// <summary>
        /// Gets or sets the href value of the navigational element.
        /// </summary>
        /// <value>
        /// The href value.
        /// </value>
        string Href { get; }

        int Depth { get; }


        Func<INavigationElement<TKey>, string> IconCssClass { get; }
    }
}