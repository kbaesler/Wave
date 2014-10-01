using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;

using Miner.Framework.Search;

namespace Miner.Framework.Trace
{
    /// <summary>
    ///     A connectivity trace that builds an <see cref="AdjacencyGraph{TVertex,TEdge}" /> of the connected edges and
    ///     junctions.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AdjacencyTrace : AdjacencyTrace<AdjacencyNode, AdjacencyEidResults>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyTrace" /> class.
        /// </summary>
        public AdjacencyTrace()
            : this(true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyTrace" /> class.
        /// </summary>
        /// <param name="isBuildingGraph">if set to <c>true</c> a graph representation of the results will be built.</param>
        public AdjacencyTrace(bool isBuildingGraph)
            : base(isBuildingGraph)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the strongly-typed results.
        /// </summary>
        /// <returns>
        ///     The <see cref="EidSearchResults" /> for the given type.
        /// </returns>
        protected override AdjacencyEidResults CreateResults()
        {
            AdjacencyEidResults eidResults = new AdjacencyEidResults
            {
                DrawComplex = true,
                Edges = base.GeometricNetwork.CreateEnumNetEID(esriElementType.esriETEdge, this.Edges),
                Junctions = base.GeometricNetwork.CreateEnumNetEID(esriElementType.esriETJunction, this.Junctions),
                GeometricNetwork = base.GeometricNetwork,
                Graph = this.Graph as AdjacencyGraph<IEIDInfo, AdjacencyNode>
            };

            return eidResults;
        }

        /// <summary>
        ///     Gets the node that will be added to the graph.
        /// </summary>
        /// <param name="edge">The edge feature.</param>
        /// <param name="source">The source junction feature.</param>
        /// <param name="target">The target junction feature.</param>
        /// <param name="sourceAlreadyVisited">if set to <c>true</c> if the source junction has already been visited by the trace.</param>
        /// <param name="targetAlreadyVisited">if set to <c>true</c> if the target junction has already been visited by the trace.</param>
        /// <returns>
        ///     Returns an <see cref="AdjacencyNode" /> representing the edge and junctions.
        /// </returns>
        protected override AdjacencyNode GetAdjacencyNode(IEIDInfo edge, IEIDInfo source, IEIDInfo target, bool sourceAlreadyVisited, bool targetAlreadyVisited)
        {
            return new AdjacencyNode(edge, source, target, sourceAlreadyVisited, targetAlreadyVisited);
        }

        #endregion
    }

    /// <summary>
    ///     A connectivity trace that builds an <see cref="AdjacencyGraph{TVertex, TEdge}" /> of the connected edges and
    ///     junctions.
    /// </summary>
    /// <typeparam name="TEdge">The type of the edge.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class AdjacencyTrace<TEdge, TResult> : ConnectivityTrace<TResult>
        where TEdge : AdjacencyNode
        where TResult : AdjacencyEidResults<TEdge>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyTrace{TEdge, TResult}" /> class.
        /// </summary>
        protected AdjacencyTrace()
            : this(true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyTrace{TEdge, TResult}" /> class.
        /// </summary>
        /// <param name="isBuildingGraph">if set to <c>true</c> a graph representation of the results will be built.</param>
        protected AdjacencyTrace(bool isBuildingGraph)
        {
            this.Graph = new AdjacencyGraph<IEIDInfo, TEdge>();
            this.Queue = new AdjacencyQueue<IEIDInfo, IEIDInfo>();
            this.IsBuildingGraph = isBuildingGraph;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the graph.
        /// </summary>
        protected IGraph<IEIDInfo, TEdge> Graph { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="Graph" /> will be built.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="Graph" /> will be built; otherwise, <c>false</c>.
        /// </value>
        protected bool IsBuildingGraph { get; set; }

        /// <summary>
        ///     Gets or sets the queue.
        /// </summary>
        /// <value>
        ///     The queue.
        /// </value>
        protected AdjacencyQueue<IEIDInfo, IEIDInfo> Queue { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the node that will be added to the graph.
        /// </summary>
        /// <param name="edge">The edge feature.</param>
        /// <param name="source">The source junction feature.</param>
        /// <param name="target">The target junction feature.</param>
        /// <param name="sourceAlreadyVisited">if set to <c>true</c> if the source junction has already been visited by the trace.</param>
        /// <param name="targetAlreadyVisited">if set to <c>true</c> if the target junction has already been visited by the trace.</param>
        /// <returns>
        ///     Returns an <see cref="AdjacencyNode" /> representing the edge and junctions.
        /// </returns>
        protected abstract TEdge GetAdjacencyNode(IEIDInfo edge, IEIDInfo source, IEIDInfo target, bool sourceAlreadyVisited, bool targetAlreadyVisited);

        /// <summary>
        ///     Called after the trace has completed. The default behavior is to do nothing.
        /// </summary>
        /// <param name="traceResults">The trace results.</param>
        /// <param name="feature">The feature.</param>
        protected override void OnAfterTrace(TResult traceResults, IFeature feature)
        {
            base.OnAfterTrace(traceResults, feature);

            this.Queue.Clear();
        }

        /// <summary>
        ///     Called before the trace has started. The default behavior is to initialize instance variables.
        /// </summary>
        /// <param name="feature">The feature.</param>
        protected override void OnBeforeTrace(IFeature feature)
        {
            base.OnBeforeTrace(feature);

            this.Graph.Clear();
            this.Queue.Clear();
        }

        /// <summary>
        ///     Called when a connected edge is traced.
        /// </summary>
        /// <param name="edgeEID">The edge EID.</param>
        /// <param name="edgeIndex">Index of the edge.</param>
        /// <param name="edgeCount">The edge count.</param>
        /// <param name="junctionEID">The junction EID.</param>
        /// <returns>
        ///     <c>true</c> to continue tracing; otherwise <c>false</c> to stop the trace.
        /// </returns>
        protected override bool OnConnectedEdge(int edgeEID, int edgeIndex, int edgeCount, int junctionEID)
        {
            if (this.IsBuildingGraph)
            {
                IEIDInfo edge = base.GetEIDInfo(edgeEID, esriElementType.esriETEdge);
                this.Queue.Edges.Enqueue(edge);
            }

            return (edgeEID > 0);
        }

        /// <summary>
        ///     Called when a connected junction is traced.
        /// </summary>
        /// <param name="junctionEID">The junction EID.</param>
        /// <param name="edgeEID">The edge EID.</param>
        /// <returns>
        ///     <c>true</c> to continue tracing; otherwise <c>false</c> to stop the trace.
        /// </returns>
        protected override bool OnConnectedJunction(int junctionEID, int edgeEID)
        {
            if (this.IsBuildingGraph)
            {
                IEIDInfo target = base.GetEIDInfo(junctionEID, esriElementType.esriETJunction);
                if (this.Queue.Vertices.Count == 0)
                {
                    this.Queue.Vertices.Enqueue(target);
                }
                else if (this.Queue.Edges.Count > 0)
                {
                    // Dequeue the source (or From) junction that has been already been visited.
                    IEIDInfo source = this.Queue.Vertices.Dequeue();

                    // Get the edge that connects the two junctions.
                    IEIDInfo edge = this.Queue.Edges.Dequeue();

                    // Add the edge to the graph.
                    this.AddEdge(source, target, edge);

                    // Add the target junction onto the queue.
                    this.Queue.Vertices.Enqueue(target);
                }
            }

            return (junctionEID > 0);
        }

        /// <summary>
        ///     Called when the trace has finished tracing a branch and is being reset back to the branching vertex.
        /// </summary>
        /// <param name="junctionEID">The junction EID.</param>
        /// <param name="edgeEID">The edge EID.</param>
        /// <param name="edgeCount">The edge count.</param>
        protected override void OnReset(int junctionEID, int edgeEID, int edgeCount)
        {
            if (this.IsBuildingGraph)
            {
                IEIDInfo target = base.GetEIDInfo(junctionEID, esriElementType.esriETJunction);
                this.Queue.Vertices.Clear();
                this.Queue.Vertices.Enqueue(target);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds the edge to the graph.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="edge">The edge.</param>
        private void AddEdge(IEIDInfo source, IEIDInfo target, IEIDInfo edge)
        {
            int visitedCount;

            // Determine if the source has been visited more than once (because we already visited it once, since it's in the queue).
            bool sourceAlreadyVisited = this.AlreadyVisited(source, esriElementType.esriETJunction, out visitedCount);
            if (visitedCount == 1) sourceAlreadyVisited = false;

            // Determine if the target has already been visited.
            bool targetAlreadyVisited = this.AlreadyVisited(target, esriElementType.esriETJunction, out visitedCount);

            // Add the adjacency edge to the graph.
            TEdge adjacencyEdge = this.GetAdjacencyNode(edge, source, target, sourceAlreadyVisited, targetAlreadyVisited);
            this.Graph.AddEdge(adjacencyEdge);
        }

        /// <summary>
        ///     Determines whether the specified element has already been visited by the trace.
        /// </summary>
        /// <param name="eidInfo">The eid information.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="visitedCount">The number of times the eid has been visited.</param>
        /// <returns>
        ///     <c>true</c> if the specified element has already been visited by the trace; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters")]
        private bool AlreadyVisited(IEIDInfo eidInfo, esriElementType elementType, out int visitedCount)
        {
            visitedCount = 0;

            if (elementType == esriElementType.esriETEdge)
                visitedCount = base.Edges.Count(e => e == eidInfo.EID);
            else if (elementType == esriElementType.esriETJunction)
                visitedCount = base.Junctions.Count(e => e == eidInfo.EID);

            return visitedCount > 0;
        }

        #endregion
    }
}