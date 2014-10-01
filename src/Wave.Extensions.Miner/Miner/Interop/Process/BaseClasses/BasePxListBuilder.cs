namespace Miner.Interop.Process
{
    /// <summary>
    ///     An abstract builder used to create custom build objects in the Process Framework.
    /// </summary>
    public abstract class BasePxListBuilder : IMMListBuilder, IMMPxListBuilderInit
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxListBuilder" /> class.
        /// </summary>
        /// <param name="builderName">Name of the builder.</param>
        protected BasePxListBuilder(string builderName)
        {
            this.Name = builderName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxListBuilder" /> class.
        /// </summary>
        /// <param name="builderName">Name of the builder.</param>
        /// <param name="pxApp">The process application refernece.</param>
        protected BasePxListBuilder(string builderName, IMMPxApplication pxApp)
            : this(builderName)
        {
            this.Name = builderName;
            this.PxApplication = pxApp;
        }

        #endregion

        #region IMMListBuilder Members

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        ///     Performs the queries necessary to build the list.
        /// </summary>
        /// <param name="pList">The list.</param>
        public abstract void BuildList(ID8List pList);

        /// <summary>
        ///     Updates the node with the specified <paramref name="vData" />.
        /// </summary>
        /// <param name="vData">The data.</param>
        /// <param name="pListItem">The list item.</param>
        /// <remarks>
        ///     Allows the user to update the list in the tree based on changes made to the node(s).
        ///     For example, if a user changes ownership or the state of a node, this method can update the list accordingly.
        /// </remarks>
        public virtual void UpdateNode(ref object vData, ID8ListItem pListItem)
        {
        }

        #endregion

        #region IMMPxListBuilderInit Members

        /// <summary>
        ///     Sets the process framework application reference.
        /// </summary>
        /// <value>
        ///     The process framework application reference.
        /// </value>
        public IMMPxApplication PxApplication { set; protected get; }

        #endregion
    }
}