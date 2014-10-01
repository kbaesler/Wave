using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    ///     Provides the properties and methods to represent a hierarchical node.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
    public interface IHierarchy<TValue> : IRecursion<TValue>
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the children.
        /// </summary>
        /// <value>The child nodes.</value>
        IEnumerable<IHierarchy<TValue>> Children { get; set; }

        /// <summary>
        ///     Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        TValue Parent { get; set; }

        #endregion
    }
}