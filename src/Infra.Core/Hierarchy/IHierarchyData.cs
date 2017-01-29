namespace Borg.Infra.Hierarchy
{
    /// <summary>
    /// Exposes a node of a hierarchical data structure, including the node object and some properties that describe characteristics of the node. Objects that implement the <see cref="T:Borg.Infra.Hierarchy.IHierarchyData"/> interface can be contained in <see cref="T:Borg.Infra.Hierarchy.IHierarchicalEnumerable"/> collections, and are used by ASP.NET site navigation and data source controls.
    /// </summary>
    public interface IHierarchyData
    {
        /// <summary>
        /// Indicates whether the hierarchical data node that the <see cref="T:Borg.Infra.Hierarchy.IHierarchyData"/> object represents has any child nodes.
        /// </summary>
        ///
        /// <returns>
        /// true if the current node has child nodes; otherwise, false.
        /// </returns>
        bool HasChildren { get; }

        /// <summary>
        /// Gets the hierarchical data node that the <see cref="T:Borg.Infra.Hierarchy.IHierarchyData"/> object represents.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="T:System.Object"/> hierarchical data node object.
        /// </returns>
        object Item { get; }

        /// <summary>
        /// Gets the name of the type of <see cref="T:System.Object"/> contained in the <see cref="P:Borg.Infra.Hierarchy.IHierarchyData.Item"/> property.
        /// </summary>
        ///
        /// <returns>
        /// The name of the type of object that the <see cref="T:Borg.Infra.Hierarchy.IHierarchyData"/> object represents.
        /// </returns>
        string Type { get; }

        /// <summary>
        /// Gets an enumeration object that represents all the child nodes of the current hierarchical node.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="T:Borg.Infra.Hierarchy.IHierarchicalEnumerable"/> collection of child nodes of the current hierarchical node.
        /// </returns>
        IHierarchicalEnumerable Children { get; }

        /// <summary>
        /// Gets an <see cref="T:Borg.Infra.Hierarchy.IHierarchyData"/> object that represents the parent node of the current hierarchical node.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="T:Borg.Infra.Hierarchy.IHierarchyData"/> object that represents the parent node of the current hierarchical node.
        /// </returns>
        IHierarchyData Parent { get; }
    }
}