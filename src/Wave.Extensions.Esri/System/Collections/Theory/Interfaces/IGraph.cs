using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     A graph with vertices of type <typeparamref name="TVertex" />
    ///     and edges of type <typeparamref name="TEdge" />
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    [ComVisible(false)]
    public interface IGraph<TVertex, TEdge> : IEdgeSet<TVertex, TEdge>, IVertexSet<TVertex>
        where TEdge : IEdge<TVertex>
    {
        #region Public Methods

        /// <summary>
        ///     Clears the vertex and edges
        /// </summary>
        void Clear();

        #endregion
    }
}