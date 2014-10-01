using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     A set of edges
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    /// <typeparam name="TEdge">The type of the edge.</typeparam>
    [ComVisible(false)]
    public interface IEdgeSet<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        #region Events

        /// <summary>
        ///     Raised when an edge is added to the graph.
        /// </summary>
        event EventHandler<EdgeEventArgs<TVertex, TEdge>> EdgeAdded;

        /// <summary>
        ///     Raised when an edge has been removed from the graph.
        /// </summary>
        event EventHandler<EdgeEventArgs<TVertex, TEdge>> EdgeRemoved;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the edge count.
        /// </summary>
        /// <value>The edge count.</value>
        int EdgeCount { get; }

        /// <summary>
        ///     Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        IEnumerable<TEdge> Edges { get; }

        /// <summary>
        ///     Gets a value indicating whether there are no edges in this set.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this set is empty; otherwise, <c>false</c>.
        /// </value>
        bool IsEdgesEmpty { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds the edge to the graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns><c>true</c> if the edge was added, otherwise <c>false</c>.</returns>
        bool AddEdge(TEdge edge);

        /// <summary>
        ///     Adds a set of edges to the graph.
        /// </summary>
        /// <param name="edges"></param>
        /// <returns>The number of edges successfully added to the graph.</returns>
        int AddEdgeRange(IEnumerable<TEdge> edges);

        /// <summary>
        ///     Determines whether the specified edge contains edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns>
        ///     <c>true</c> if the specified edge contains edge; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsEdge(TEdge edge);

        /// <summary>
        ///     Determines whether the specified set contains both the vertices.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///     <c>true</c> if the specified set contains both the vertices; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsEdge(TVertex source, TVertex target);

        /// <summary>
        ///     Removes <paramref name="edge" /> from the graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns><c>true</c> if <paramref name="edge" /> was successfully removed; otherwise <c>false</c>.</returns>
        bool RemoveEdge(TEdge edge);

        /// <summary>
        ///     Removes all edges that match <paramref name="predicate" />.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="predicate">
        ///     A pure delegate that takes an <typeparamref name="TEdge" /> and returns true if the edge should
        ///     be removed.
        /// </param>
        /// <returns>The number of edges removed.</returns>
        int RemoveEdge(TVertex vertex, Func<TEdge, bool> predicate);

        #endregion
    }

    /// <summary>
    ///     An event involving an edge.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    /// <typeparam name="TEdge">The type of the edge.</typeparam>
    [ComVisible(false)]
    public class EdgeEventArgs<TVertex, TEdge> : EventArgs
        where TEdge : IEdge<TVertex>
    {
        #region Fields

        private readonly TEdge _Edge;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EdgeEventArgs&lt;TVertex, TEdge&gt;" /> class.
        /// </summary>
        /// <param name="edge">The edge.</param>
        public EdgeEventArgs(TEdge edge)
        {
            _Edge = edge;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the edge.
        /// </summary>
        /// <value>The edge.</value>
        public TEdge Edge
        {
            get { return _Edge; }
        }

        #endregion
    }
}