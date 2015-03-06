namespace Miner.Interop.Process
{
    /// <summary>
    ///     A contract used for constructing a process framework entity object.
    /// </summary>
    public interface IPxNode
    {
        #region Public Properties

        /// <summary>
        ///     Gets the history.
        /// </summary>
        IMMPxNodeHistory History { get; }

        /// <summary>
        ///     Gets the ID.
        /// </summary>
        int ID { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxNode" /> is open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if open; otherwise, <c>false</c>.
        /// </value>
        bool IsOpen { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="IPxNode" /> is locked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if locked; otherwise, <c>false</c>.
        /// </value>
        bool Locked { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        ///     Gets the node.
        /// </summary>
        IMMPxNode Node { get; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        IMMPxState State { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IPxNode" /> is valid.
        /// </summary>
        /// <value>
        ///     <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        bool Valid { get; }

        /// <summary>
        ///     Gets the name of the version.
        /// </summary>
        /// <value>
        ///     The name of the version.
        /// </value>
        string VersionName { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds a new history record for the node using the
        ///     specified <paramref name="description" /> and <paramref name="extraData" />.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="extraData">The extra data.</param>
        void AddHistory(string description, string extraData);

        /// <summary>
        ///     Creates the process framework node for the specified the <paramref name="user" />.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully created; otherwise <c>false</c>.
        /// </returns>
        bool CreateNew(IMMPxUser user);

        /// <summary>
        ///     Deletes the <see cref="IMMPxNode" /> from the process framework database.
        /// </summary>
        void Delete();

        /// <summary>
        ///     Initializes the process framework node using the specified <paramref name="nodeID" /> for the node.
        /// </summary>
        /// <param name="nodeID">The node ID.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully initialized; otherwise
        ///     <c>false</c>.
        /// </returns>
        bool Initialize(int nodeID);

        /// <summary>
        ///     Sets node as the current node.
        /// </summary>
        void Set();

        /// <summary>
        ///     Updates the entity by flushing the <see cref="IMMPxNode" /> to the database and reinitializing the
        ///     <see cref="IMMPxNode" />.
        /// </summary>
        void Update();

        #endregion
    }
}