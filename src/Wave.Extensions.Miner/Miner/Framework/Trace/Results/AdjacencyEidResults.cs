using System.Collections;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.NetworkAnalysis;

using Miner.Framework.Search;

namespace Miner.Framework.Trace
{
    /// <summary>
    ///     A supporting class used to contain the memory graph of the results from a trace using the
    ///     <see cref="AdjacencyTrace" /> strategy.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Eid"), ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AdjacencyEidResults : AdjacencyEidResults<AdjacencyNode>
    {
    }

    /// <summary>
    ///     A supporting class used to contain the memory graph of the results from a trace using the
    ///     <see cref="AdjacencyTrace" /> strategy.
    /// </summary>
    /// <typeparam name="TEdge">The type of the edge.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Eid"), ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class AdjacencyEidResults<TEdge> : EidSearchResults
        where TEdge : AdjacencyNode
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyEidResults&lt;TEdge&gt;" /> class.
        /// </summary>
        protected AdjacencyEidResults()
        {
            this.Graph = new AdjacencyGraph<IEIDInfo, TEdge>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the graph.
        /// </summary>
        /// <value>
        ///     The graph.
        /// </value>
        public AdjacencyGraph<IEIDInfo, TEdge> Graph { get; set; }

        #endregion
    }
}