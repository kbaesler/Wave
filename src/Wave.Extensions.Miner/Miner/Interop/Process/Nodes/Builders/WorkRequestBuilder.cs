using System.Collections.Generic;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     A list builder that can be used to build work request nodes.
    /// </summary>
    public class WorkRequestBuilder : BasePxNodeBuilder
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkRequestBuilder" /> class.
        /// </summary>
        /// <param name="pxApp">The process application refernece.</param>
        /// <param name="nodeIDs">The node IDs.</param>
        public WorkRequestBuilder(IMMPxApplication pxApp, IList<int> nodeIDs)
            : base("Work Request Builder", pxApp, nodeIDs, WorkRequest.NodeTypeName)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the node with the specified <paramref name="nodeID" />.
        /// </summary>
        /// <param name="nodeID">The node ID.</param>
        /// <returns>
        ///     Returns the <see cref="Miner.Interop.Process.IMMPxNode" /> representing the node.
        /// </returns>
        protected override IMMPxNode GetNode(int nodeID)
        {
            var o = new WorkRequest(base.PxApplication);
            return (o.Initialize(nodeID)) ? o.Node : null;
        }

        #endregion
    }
}