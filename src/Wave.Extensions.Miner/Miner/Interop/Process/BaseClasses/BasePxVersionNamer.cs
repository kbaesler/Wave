using System.Runtime.InteropServices;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     The Version Namer object determines the naming convention of versions associated with a node in the Process
    ///     Framework.
    /// </summary>
    [ComVisible(true)]
    public abstract class BasePxVersionNamer : IMMPxSDEVersionNamer, IMMPxDisplayName
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxVersionNamer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BasePxVersionNamer(string name)
        {
            this.DisplayName = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName { get; protected set; }

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        /// <value>
        ///     The application.
        /// </value>
        public IMMPxApplication PxApplication { set; protected get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the name of the base version.
        /// </summary>
        /// <param name="lNodeID">The node identifier.</param>
        /// <returns>Returns a <see cref="string" /> representing the name of the version.</returns>
        public abstract string GetBaseVersionName(int lNodeID);

        /// <summary>
        ///     Gets the name of the version.
        /// </summary>
        /// <param name="lNodeID">The node identifier.</param>
        /// <returns>Returns a <see cref="string" /> representing the name of the version.</returns>
        public abstract string GetVersionName(int lNodeID);

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            MMProcessMgrNamer.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMProcessMgrNamer.Unregister(registryKey);
        }

        #endregion
    }
}