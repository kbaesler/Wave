using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.NetworkAnalysis;

namespace Miner.Framework.Trace
{
    /// <summary>
    ///     A geometric network edge that represents an edge connected by two junctions.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AdjacencyNode : Edge<IEIDInfo>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdjacencyNode" /> class.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <param name="source">The source (or from) junction.</param>
        /// <param name="target">The target (or to) junction.</param>
        /// <param name="sourceAlreadyVisited">if set to <c>true</c> if the source junction parcipates in a loop.</param>
        /// <param name="targetAlreadyVisited">if set to <c>true</c> if the target junction parcipates in a loop.</param>
        public AdjacencyNode(IEIDInfo edge, IEIDInfo source, IEIDInfo target, bool sourceAlreadyVisited, bool targetAlreadyVisited)
            : base(source, target)
        {
            this.Edge = edge;
            this.SourceAlreadyVisited = sourceAlreadyVisited;
            this.TargetAlreadyVisited = targetAlreadyVisited;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the edge.
        /// </summary>
        public IEIDInfo Edge { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Miner.Theory.Edge{TVertex}.Source" /> junction participates in a
        ///     loop.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="Miner.Theory.Edge{TVertex}.Source" /> junction participates in a loop; otherwise,
        ///     <c>false</c>.
        /// </value>
        public bool SourceAlreadyVisited { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Miner.Theory.Edge{TVertex}.Target" /> junction participates in a
        ///     loop.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="Miner.Theory.Edge{TVertex}.Target" /> jnuction participates in a loop; otherwise,
        ///     <c>false</c>.
        /// </value>
        public bool TargetAlreadyVisited { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        ///     The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            AdjacencyNode other = obj as AdjacencyNode;
            if (other == null) return false;

            return (other.Source.Feature.OID.Equals(this.Source.Feature.OID) && other.Source.Feature.Class.ObjectClassID.Equals(this.Source.Feature.Class.ObjectClassID)
                    && other.Target.Feature.OID.Equals(this.Target.Feature.OID) && other.Target.Feature.Class.ObjectClassID.Equals(this.Target.Feature.Class.ObjectClassID));
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return new {A = SourceAlreadyVisited, B = TargetAlreadyVisited, C = Target, D = Source, E = Edge}.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}->{1}", this.Source.Feature.OID, this.Target.Feature.OID);
        }

        #endregion
    }
}