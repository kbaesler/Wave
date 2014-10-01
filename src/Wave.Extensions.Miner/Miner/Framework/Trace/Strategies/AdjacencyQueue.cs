using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Miner.Framework.Trace
{
    /// <summary>
    ///     A queue of edges and vertices for the a graph.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AdjacencyQueue<TVertex, TEdge>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyQueue&lt;TVertex, TEdge&gt;" /> class.
        /// </summary>
        public AdjacencyQueue()
        {
            this.Edges = new Queue<TEdge>();
            this.Vertices = new Queue<TVertex>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the edges.
        /// </summary>
        public Queue<TEdge> Edges { get; private set; }

        /// <summary>
        ///     Gets the vertices.
        /// </summary>
        public Queue<TVertex> Vertices { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Clears the queues.
        /// </summary>
        public void Clear()
        {
            this.Edges.Clear();
            this.Vertices.Clear();
        }

        #endregion
    }
}