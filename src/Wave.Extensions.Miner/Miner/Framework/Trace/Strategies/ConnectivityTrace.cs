using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Framework.Search;
using Miner.Interop;

namespace Miner.Framework.Trace
{
    /// <summary>
    ///     A connectivity trace that keeps track of all the junctions and edges traced.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ConnectivityTrace : ConnectivityTrace<EidSearchResults>
    {
        #region Protected Methods

        /// <summary>
        ///     Creates the strongly-typed results.
        /// </summary>
        /// <returns>
        ///     The <see cref="EidSearchResults" /> for the given type.
        /// </returns>
        protected override EidSearchResults CreateResults()
        {
            EidSearchResults eidResults = new EidSearchResults
            {
                DrawComplex = true,
                Edges = base.GeometricNetwork.CreateEnumNetEID(esriElementType.esriETEdge, this.Edges),
                Junctions = base.GeometricNetwork.CreateEnumNetEID(esriElementType.esriETJunction, this.Junctions),
                GeometricNetwork = base.GeometricNetwork
            };

            return eidResults;
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
            return (junctionEID > 0);
        }

        #endregion
    }

    /// <summary>
    ///     A connectivity trace that keeps track of all the junctions and edges traced.
    /// </summary>
    /// <typeparam name="TResults">The type of the results.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class ConnectivityTrace<TResults> : BaseTrace<TResults>
        where TResults : EidSearchResults
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConnectivityTrace&lt;TResults&gt;" /> class.
        /// </summary>
        protected ConnectivityTrace()
        {
            this.Edges = new List<int>();
            this.Junctions = new List<int>();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the edges that have been traced.
        /// </summary>
        /// <value>The edges.</value>
        protected List<int> Edges { get; set; }

        /// <summary>
        ///     Gets the junctions that have been traced.
        /// </summary>
        /// <value>The junctions.</value>
        protected List<int> Junctions { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the <see cref="IForwardStarGEN" /> that will be used to navigate the <paramref name="network" />.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>
        /// The <see cref="IForwardStarGEN" /> that is used to navigate the network.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">network</exception>
        protected virtual IForwardStarGEN CreateNavigator(INetwork network)
        {
            if(network == null) throw new ArgumentNullException("network");

            return (IForwardStarGEN) network.CreateForwardStar(false, null, null, null, null);
        }

        /// <summary>
        ///     Creates the strongly-typed results.
        /// </summary>
        /// <returns>
        ///     The <see cref="EidSearchResults" /> for the given type.
        /// </returns>
        protected abstract TResults CreateResults();

        /// <summary>
        ///     Returns the weight value of the specified network element for the specified weight.
        /// </summary>
        /// <param name="eid">The eid.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="weightID">The weight ID.</param>
        /// <returns>
        ///     Returns the weight value of the specified network element for the specified weight.
        /// </returns>
        protected int GetWeight(int eid, esriElementType elementType, int weightID)
        {
            if (base.GeometricNetwork == null || weightID < 0)
                return -1;

            INetAttributes netAttributes = (INetAttributes) base.GeometricNetwork.Network;
            object value = netAttributes.GetWeightValue(eid, elementType, weightID);
            return TypeCast.Cast(value, -1);
        }

        /// <summary>
        ///     Called after the <see cref="OnTrace(IFeature)" /> method has executed.
        /// </summary>
        /// <param name="traceResults">The results.</param>
        /// <param name="feature">The feature.</param>
        protected override void OnAfterTrace(TResults traceResults, IFeature feature)
        {
            this.Edges.Clear();
            this.Junctions.Clear();
        }

        /// <summary>
        ///     Called before the trace executes the <see cref="OnTrace(IFeature)" /> method.
        /// </summary>
        /// <param name="feature">The feature.</param>
        protected override void OnBeforeTrace(IFeature feature)
        {
            this.Edges.Clear();
            this.Junctions.Clear();

            base.OnBeforeTrace(feature);
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
        protected abstract bool OnConnectedEdge(int edgeEID, int edgeIndex, int edgeCount, int junctionEID);

        /// <summary>
        ///     Called when a connected junction is traced.
        /// </summary>
        /// <param name="junctionEID">The junction EID.</param>
        /// <param name="edgeEID">The edge EID.</param>
        /// <returns>
        ///     <c>true</c> to continue tracing; otherwise <c>false</c> to stop the trace.
        /// </returns>
        protected abstract bool OnConnectedJunction(int junctionEID, int edgeEID);

        /// <summary>
        ///     Called when the trace has finished tracing a branch and is being reset back to the branching vertex.
        /// </summary>
        /// <param name="junctionEID">The junction EID.</param>
        /// <param name="edgeEID">The edge EID.</param>
        /// <param name="edgeCount">The edge count.</param>
        protected virtual void OnReset(int junctionEID, int edgeEID, int edgeCount)
        {
        }

        /// <summary>
        ///     Initiates the tracing strategy given the provided feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns>
        ///     The <see cref="IMMSearchResults" /> containing the results from the trace.
        /// </returns>
        protected override TResults OnTrace(IFeature feature)
        {
            if (base.GeometricNetwork == null)
                return null;

            IForwardStarGEN fstar = this.CreateNavigator(base.GeometricNetwork.Network);
            if (base.ElementType == esriElementType.esriETJunction)
            {
                this.FindConnected(fstar, base.EID, -1);
            }
            else
            {
                IComplexEdgeFeature cef = (IComplexEdgeFeature) feature;
                for (int j = 0; j < cef.JunctionFeatureCount; j++)
                {
                    ISimpleJunctionFeature sjf = (ISimpleJunctionFeature) cef.get_JunctionFeature(j);
                    this.FindConnected(fstar, sjf.EID, base.EID);
                }
            }

            // Return the strongly-typed results.
            return this.CreateResults();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Finds the connected features using a normal connectivity trace.
        /// </summary>
        /// <param name="fstar">The fstar.</param>
        /// <param name="junctionEID">The junction EID.</param>
        /// <param name="edgeEID">The edge EID.</param>
        private void FindConnected(IForwardStarGEN fstar, int junctionEID, int edgeEID)
        {
            // Raise the connected junction event.
            if (!this.OnConnectedJunction(junctionEID, edgeEID))
                return;

            // Add the junction to the collection.
            if (!this.Junctions.Contains(junctionEID))
                this.Junctions.Add(junctionEID);

            // Trace using the specified junction.
            int edgeCount;
            fstar.FindAdjacent(0, junctionEID, out edgeCount);

            int[] edgeIDs = new int[edgeCount];
            bool[] reverseOrientation = new bool[edgeCount];
            object[] weights = new object[edgeCount];

            fstar.QueryAdjacentEdges(ref edgeIDs, ref reverseOrientation, ref weights);
            for (int edgeIndex = 0; edgeIndex < edgeCount; edgeIndex++)
            {
                int newAdjacentEdgeEID = edgeIDs[edgeIndex];
                if (this.Edges.Contains(newAdjacentEdgeEID))
                    continue;

                // Notfy that the recursive junction should be reset.
                if (edgeIndex > 0 && edgeCount > 2)
                    this.OnReset(junctionEID, edgeEID, edgeCount);

                // Raise the connected edge event.
                if (!this.OnConnectedEdge(newAdjacentEdgeEID, edgeIndex, edgeCount, junctionEID))
                    continue;

                // Add the junction to the collection.
                this.Edges.Add(newAdjacentEdgeEID);

                // Find the next (or adjacent) junction on this edge.
                int newEdgeCount;
                int newAdjacentJunctionEID;
                object newAdjacentJunctionWeight;

                // Reset the graph.
                fstar.FindAdjacent(0, junctionEID, out newEdgeCount);
                fstar.QueryAdjacentJunction(edgeIndex, out newAdjacentJunctionEID, out newAdjacentJunctionWeight);

                // Continue the trace from the next (or adjacent) junction.
                this.FindConnected(fstar, newAdjacentJunctionEID, newAdjacentEdgeEID);
            }
        }

        #endregion
    }
}