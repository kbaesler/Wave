using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     A set of vertices
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    [ComVisible(false)]
    public interface IVertexSet<TVertex>
    {
        #region Events

        /// <summary>
        ///     Raised when an vertex is added to the graph.
        /// </summary>
        event EventHandler<VertexEventArgs<TVertex>> VertexAdded;

        /// <summary>
        ///     Raised when an vertex has been removed from the graph.
        /// </summary>
        event EventHandler<VertexEventArgs<TVertex>> VertexRemoved;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether there are no vertices in this set.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the vertex set is empty; otherwise, <c>false</c>.
        /// </value>
        bool IsVerticesEmpty { get; }

        /// <summary>
        ///     Gets the vertex count.
        /// </summary>
        /// <value>The vertex count.</value>
        int VertexCount { get; }

        /// <summary>
        ///     Gets the vertices.
        /// </summary>
        /// <value>The vertices.</value>
        IEnumerable<TVertex> Vertices { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds the vertex to the graph
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        ///     <c>true</c> if the vertex was added, otherwise <c>false</c>.
        /// </returns>
        bool AddVertex(TVertex vertex);

        /// <summary>
        ///     Adds a set of vertices to the graph.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <returns>
        ///     The number of vertices successfully added to the graph.
        /// </returns>
        int AddVertexRange(IEnumerable<TVertex> vertices);

        /// <summary>
        ///     Determines whether the specified vertex contains vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        ///     <c>true</c> if the specified vertex contains vertex; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsVertex(TVertex vertex);

        /// <summary>
        ///     Removes <paramref name="vertex" /> from the graph
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="vertex" /> was successfully removed; otherwise <c>false</c>.
        /// </returns>
        bool RemoveVertex(TVertex vertex);

        /// <summary>
        ///     Removes all edges that match <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">
        ///     A pure delegate that takes an <typeparamref name="TVertex" /> and returns true if the edge
        ///     should be removed.
        /// </param>
        /// <returns>The number of vertices removed.</returns>
        int RemoveVertex(Func<TVertex, bool> predicate);

        #endregion
    }

    /// <summary>
    ///     An event involving a vertex.
    /// </summary>
    /// <typeparam name="TVertex">The type of the vertex.</typeparam>
    [ComVisible(false)]
    public class VertexEventArgs<TVertex> : EventArgs
    {
        #region Fields

        private readonly TVertex _Vertex;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VertexEventArgs&lt;TVertex&gt;" /> class.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        public VertexEventArgs(TVertex vertex)
        {
            _Vertex = vertex;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the vertex.
        /// </summary>
        /// <value>The vertex.</value>
        public TVertex Vertex
        {
            get { return _Vertex; }
        }

        #endregion
    }
}