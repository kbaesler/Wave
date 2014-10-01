using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel
{
    /// <summary>
    ///     An observable collection of edges and vertices.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    /// <typeparam name="TEdge">The type of the edge.</typeparam>
    [ComVisible(false)]
    public sealed class EdgeCollection<TVertex, TEdge> : Collection<TEdge>
        where TEdge : IEdge<TVertex>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EdgeCollection&lt;TVertex, TEdge&gt;" /> class.
        /// </summary>
        public EdgeCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EdgeCollection&lt;TVertex, TEdge&gt;" /> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public EdgeCollection(EdgeCollection<TVertex, TEdge> list)
            : base(list)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns></returns>
        public EdgeCollection<TVertex, TEdge> Clone()
        {
            return new EdgeCollection<TVertex, TEdge>(this);
        }

        #endregion
    }
}