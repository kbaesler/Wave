using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Collections
{
    /// <summary>
    ///     A mutable directed graph data structure efficient for sparse
    ///     graph representation where out-edge need to be enumerated only.
    /// </summary>
    /// <typeparam name="TVertex">type of the vertices</typeparam>
    /// <typeparam name="TEdge">type of the edges</typeparam>
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    [ComVisible(false)]
    public class AdjacencyGraph<TVertex, TEdge> : IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyGraph&lt;TVertex, TEdge&gt;" /> class.
        /// </summary>
        public AdjacencyGraph()
        {
            this.EdgeList = new List<TEdge>();
            this.VertexEdges = new VertexEdgeDictionary<TVertex, TEdge>();
            this.VertexEdges.DictionaryChanged += OnDictionaryChanged;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the index edges.
        /// </summary>
        /// <value>
        ///     The index edges.
        /// </value>
        protected IList<TEdge> EdgeList { get; private set; }

        /// <summary>
        ///     Gets or sets the vertex edges.
        /// </summary>
        /// <value>The vertex edges.</value>
        protected VertexEdgeDictionary<TVertex, TEdge> VertexEdges { get; private set; }

        #endregion

        #region IGraph<TVertex,TEdge> Members

        /// <summary>
        ///     Raised when an edge is added to the graph.
        /// </summary>
        public event EventHandler<EdgeEventArgs<TVertex, TEdge>> EdgeAdded;

        /// <summary>
        ///     Raised when an edge has been removed from the graph.
        /// </summary>
        public event EventHandler<EdgeEventArgs<TVertex, TEdge>> EdgeRemoved;

        /// <summary>
        ///     Raised when an vertex is added to the graph.
        /// </summary>
        public event EventHandler<VertexEventArgs<TVertex>> VertexAdded;

        /// <summary>
        ///     Raised when an vertex has been removed from the graph.
        /// </summary>
        public event EventHandler<VertexEventArgs<TVertex>> VertexRemoved;

        /// <summary>
        ///     Gets the edge count.
        /// </summary>
        /// <value>The edge count.</value>
        public int EdgeCount
        {
            get { return this.VertexEdges.Values.Sum(edges => edges.Count); }
        }

        /// <summary>
        ///     Gets the edges.
        /// </summary>
        /// <value>The edges.</value>
        public IEnumerable<TEdge> Edges
        {
            get { return this.EdgeList; }
        }

        /// <summary>
        ///     Gets a value indicating whether there are no edges in this set.
        /// </summary>
        /// <value><c>true</c> if this set is empty; otherwise, <c>false</c>.</value>
        public bool IsEdgesEmpty
        {
            get { return this.EdgeCount == 0; }
        }

        /// <summary>
        ///     Gets a value indicating whether there are no vertices in this set.
        /// </summary>
        /// <value><c>true</c> if the vertex set is empty; otherwise, <c>false</c>.</value>
        public bool IsVerticesEmpty
        {
            get { return this.VertexCount == 0; }
        }

        /// <summary>
        ///     Gets the vertex count.
        /// </summary>
        /// <value>The vertex count.</value>
        public int VertexCount
        {
            get { return this.VertexEdges.Count(o => !Equals(o.Key, null)); }
        }

        /// <summary>
        ///     Gets the vertices.
        /// </summary>
        /// <value>The vertices.</value>
        public IEnumerable<TVertex> Vertices
        {
            get { return this.VertexEdges.Keys; }
        }

        /// <summary>
        ///     Adds the edge to the graph
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns>
        ///     <returns><c>true</c> if the edge was added, otherwise <c>false</c>.</returns>
        /// </returns>
        public bool AddEdge(TEdge edge)
        {
            if (this.ContainsEdge(edge.Source, edge.Target))
                return false;

            this.AddVertex(edge.Source);
            this.AddVertex(edge.Target);

            this.VertexEdges[edge.Source].Add(edge);
            this.EdgeList.Add(edge);

            return true;
        }

        /// <summary>
        ///     Adds a set of edges to the graph.
        /// </summary>
        /// <param name="edges">The edges.</param>
        /// <returns>
        ///     The number of edges successfully added to the graph.
        /// </returns>
        public int AddEdgeRange(IEnumerable<TEdge> edges)
        {
            return edges.Count(this.AddEdge);
        }

        /// <summary>
        ///     Adds the vertex to the graph
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        ///     <returns><c>true</c> if the vertex was added, otherwise <c>false</c>.</returns>
        /// </returns>
        public bool AddVertex(TVertex vertex)
        {
            if (this.ContainsVertex(vertex))
                return false;

            EdgeCollection<TVertex, TEdge> edges = new EdgeCollection<TVertex, TEdge>();
            this.VertexEdges.Add(new KeyValuePair<TVertex, EdgeCollection<TVertex, TEdge>>(vertex, edges));
            return true;
        }

        /// <summary>
        ///     Adds a set of vertices to the graph.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <returns>
        ///     the number of vertices successfully added to the graph.
        /// </returns>
        public int AddVertexRange(IEnumerable<TVertex> vertices)
        {
            return vertices.Count(this.AddVertex);
        }

        /// <summary>
        ///     Clears the vertex and edges
        /// </summary>
        public void Clear()
        {
            this.VertexEdges.Clear();
            this.EdgeList.Clear();
        }

        /// <summary>
        ///     Determines whether the specified set contains both the vertices.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///     <c>true</c> if the specified set contains both the vertices; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            EdgeCollection<TVertex, TEdge> edges;
            if (!this.VertexEdges.TryGetValue(source, out edges))
                return false;

            return edges.Any(e => e.Target.Equals(target));
        }

        /// <summary>
        ///     Determines whether the specified edge contains edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns>
        ///     <c>true</c> if the specified edge contains edge; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsEdge(TEdge edge)
        {
            EdgeCollection<TVertex, TEdge> edges;
            return this.VertexEdges.TryGetValue(edge.Source, out edges) && edges.Contains(edge);
        }

        /// <summary>
        ///     Determines whether the specified vertex contains vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        ///     <c>true</c> if the specified vertex contains vertex; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsVertex(TVertex vertex)
        {
            return this.VertexEdges.ContainsKey(vertex);
        }

        /// <summary>
        ///     Removes <paramref name="edge" /> from the graph
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>
        ///     true if <paramref name="edge" /> was successfully removed; otherwise falSE.
        /// </returns>
        public bool RemoveEdge(TEdge edge)
        {
            EdgeCollection<TVertex, TEdge> edges;
            if (this.VertexEdges.TryGetValue(edge.Source, out edges) && edges.Remove(edge))
            {
                this.EdgeList.Remove(edge);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes all edges that match <paramref name="predicate" />.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="predicate">
        ///     A pure delegate that takes an
        ///     <typeparamref name="TEdge" /> and returns true if the edge should
        ///     be removed.
        /// </param>
        /// <returns>
        ///     The number of edges removed.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public int RemoveEdge(TVertex vertex, Func<TEdge, bool> predicate)
        {
            if (!this.ContainsVertex(vertex))
                return 0;

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            var edges = this.VertexEdges[vertex];
            var edgeToRemove = new EdgeCollection<TVertex, TEdge>();
            foreach (var edge in edges)
                if (predicate(edge))
                    edgeToRemove.Add(edge);

            foreach (var edge in edgeToRemove)
            {
                edges.Remove(edge);

                if (this.EdgeList.Contains(edge))
                    this.EdgeList.Remove(edge);
            }

            return edgeToRemove.Count;
        }

        /// <summary>
        ///     Removes <paramref name="vertex" /> from the graph
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns>
        ///     true if <paramref name="vertex" /> was successfully removed; otherwise falSE.
        /// </returns>
        public bool RemoveVertex(TVertex vertex)
        {
            if (!this.ContainsVertex(vertex))
                return false;

            // Remove all of the edges.
            this.VertexEdges[vertex].Clear();

            // iterage over edges and remove each edge touching the vertex
            var edgeToRemove = new EdgeCollection<TVertex, TEdge>();
            foreach (var entry in this.VertexEdges)
            {
                if (entry.Key.Equals(vertex)) continue;

                foreach (var edge in entry.Value)
                {
                    if (edge.Target.Equals(vertex))
                        edgeToRemove.Add(edge);
                }
                foreach (var edge in edgeToRemove)
                {
                    entry.Value.Remove(edge);
                }
            }

            return this.VertexEdges.Remove(vertex);
        }

        /// <summary>
        ///     Removes all edges that match <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">
        ///     A pure delegate that takes an <typeparamref name="TVertex" /> and returns true if the edge
        ///     should be removed.
        /// </param>
        /// <returns>The number of vertices removed.</returns>
        public int RemoveVertex(Func<TVertex, bool> predicate)
        {
            var vertices = new VertexCollection<TVertex>();
            foreach (var v in this.Vertices.Where(predicate))
                vertices.Add(v);

            foreach (var v in vertices)
                this.RemoveVertex(v);

            return vertices.Count;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Handles the dictionary changed event of the vertex / edges dictionary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event instance data.</param>
        /// <exception cref="System.ArgumentNullException">e</exception>
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected void OnDictionaryChanged(object sender, NotifyDictionaryChangedEventArgs<TVertex, EdgeCollection<TVertex, TEdge>> e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    if (this.EdgeAdded != null)
                    {
                        // Raise the event for each edge that was added.
                        foreach (var entry in e.NewItems)
                        {
                            foreach (var edge in entry.Value)
                            {
                                this.OnEdgeAdded(new EdgeEventArgs<TVertex, TEdge>(edge));
                            }
                        }
                    }

                    if (this.VertexAdded != null)
                    {
                        // Raise the event for the vertex.
                        this.OnVertexAdded(new VertexEventArgs<TVertex>(e.NewItem.Key));
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:

                    if (this.EdgeRemoved != null)
                    {
                        // Raise the event for each edge that was removed.
                        foreach (var entry in e.OldItems)
                        {
                            foreach (var edge in entry.Value)
                            {
                                this.OnEdgeRemoved(new EdgeEventArgs<TVertex, TEdge>(edge));
                            }
                        }
                    }

                    if (this.VertexRemoved != null)
                    {
                        // Raise the event for the vertex.
                        this.OnVertexRemoved(new VertexEventArgs<TVertex>(e.OldItem.Key));
                    }

                    break;
            }
        }

        /// <summary>
        ///     Raises the <see cref="EdgeAdded" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Collections.EdgeEventArgs&lt;TVertex,TEdge&gt;" /> instance containing the event
        ///     data.
        /// </param>
        protected void OnEdgeAdded(EdgeEventArgs<TVertex, TEdge> e)
        {
            EventHandler<EdgeEventArgs<TVertex, TEdge>> eventHandler = this.EdgeAdded;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="EdgeRemoved" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Collections.EdgeEventArgs&lt;TVertex,TEdge&gt;" /> instance containing the event
        ///     data.
        /// </param>
        protected void OnEdgeRemoved(EdgeEventArgs<TVertex, TEdge> e)
        {
            EventHandler<EdgeEventArgs<TVertex, TEdge>> eventHandler = this.EdgeRemoved;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="VertexAdded" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Collections.VertexEventArgs&lt;TVertex&gt;" /> instance containing the event
        ///     data.
        /// </param>
        protected void OnVertexAdded(VertexEventArgs<TVertex> e)
        {
            EventHandler<VertexEventArgs<TVertex>> eventHandler = this.VertexAdded;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="VertexRemoved" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Collections.VertexEventArgs&lt;TVertex&gt;" /> instance containing the event
        ///     data.
        /// </param>
        protected void OnVertexRemoved(VertexEventArgs<TVertex> e)
        {
            EventHandler<VertexEventArgs<TVertex>> eventHandler = this.VertexRemoved;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        #endregion
    }
}