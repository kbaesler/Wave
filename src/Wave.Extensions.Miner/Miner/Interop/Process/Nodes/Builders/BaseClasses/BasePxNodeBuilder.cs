using System.Collections.Generic;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An abstract list builder for building the Design, Session or Work Request nodes.
    /// </summary>
    public abstract class BasePxNodeBuilder : BasePxListBuilder
    {
        #region Fields

        private readonly IList<int> _NodeIDs;
        private readonly int _NodeTypeID;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxNodeBuilder" /> class.
        /// </summary>
        /// <param name="builderName">Name of the builder.</param>
        /// <param name="pxApp">The process application refernece.</param>
        /// <param name="nodeIDs">The node IDs.</param>
        /// <param name="nodeTypeName">The name of the node type.</param>
        protected BasePxNodeBuilder(string builderName, IMMPxApplication pxApp, IList<int> nodeIDs, string nodeTypeName)
            : base(builderName, pxApp)
        {
            _NodeIDs = nodeIDs;
            _NodeTypeID = pxApp.Helper.GetNodeTypeID(nodeTypeName);
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the node IDs.
        /// </summary>
        /// <value>
        ///     The node IDs.
        /// </value>
        protected IList<int> NodeIDs
        {
            get { return _NodeIDs; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Performs the queries necessary to build the list.
        /// </summary>
        /// <param name="pList">The list.</param>
        public override void BuildList(ID8List pList)
        {
            if (!this.Validate((IMMPxNode) pList))
                return;

            foreach (var nodeID in _NodeIDs)
            {
                IMMPxNode node = this.GetNode(nodeID);
                if (node != null)
                {
                    pList.Add((ID8ListItem) node);
                }
            }
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
        protected abstract IMMPxNode GetNode(int nodeID);

        /// <summary>
        ///     Determines if the list should be built given the list node.
        /// </summary>
        /// <param name="node">The node of the root list.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the list should be built; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool Validate(IMMPxNode node)
        {
            return node.NodeType != _NodeTypeID;
        }

        #endregion
    }
}