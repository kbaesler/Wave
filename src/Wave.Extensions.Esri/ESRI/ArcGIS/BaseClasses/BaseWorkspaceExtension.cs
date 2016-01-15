using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An abstract class used for creating a geodatabase workspace extension.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseWorkspaceExtension : IWorkspaceExtension2, IWorkspaceExtensionControl
    {
        #region Fields

        /// <summary>
        ///     Has a weak-reference to the Workspace.  Don't ever maintain a reference to the Workspace itself or else you will
        ///     have a circular reference.
        /// </summary>
        private IWorkspaceHelper _WorkspaceHelper;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseWorkspaceExtension" /> class.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        /// <param name="extensionGuid">The extension GUID.</param>
        protected BaseWorkspaceExtension(string extensionName, string extensionGuid)
        {
            this.Name = extensionName;
            this.GUID = new UIDClass {Value = extensionGuid};
        }

        #endregion

        #region IWorkspaceExtension2 Members

        /// <summary>
        ///     Gets the GUID that identifies this Workspace Extension.
        /// </summary>
        public UID GUID { get; private set; }

        /// <summary>
        ///     Gets the name for this Workspace Extension.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the workspace associated with this Workspace Extension.
        /// </summary>
        /// <remarks>Don't ever maintain a reference to the Workspace itself or else you will have a circular reference.</remarks>
        public IWorkspace Workspace
        {
            get { return _WorkspaceHelper.Workspace; }
        }

        /// <summary>
        ///     Gets any data dictionary tables that should not be exposed to browsers and should not participate in edit sessions.
        /// </summary>
        /// <remarks>
        ///     The DataDictionaryNames property return the names of tables and datasets that are private to the extension and
        ///     will not be exposed by the workspace to browsing clients.
        /// </remarks>
        public virtual IEnumBSTR DataDictionaryTableNames
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets the private datasets that should not be exposed to browsers.
        /// </summary>
        /// <param name="datasetType">Type of the dataset.</param>
        /// <returns>An enumeration of the dataset names.</returns>
        /// <remarks>
        ///     The PrivateDatasetNames property return the names of tables and datasets
        ///     that are private to the extension and will not be exposed by the workspace to browsing clients.
        /// </remarks>
        public virtual IEnumBSTR get_PrivateDatasetNames(esriDatasetType datasetType)
        {
            return null;
        }

        /// <summary>
        ///     Indicates if the workspace extension owns the dataset type.
        /// </summary>
        /// <param name="datasetType">Type of the dataset.</param>
        /// <returns>
        ///     Return <c>true</c> when the extension owns the dataset; otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     The OwnDatasetType method returns a boolean indicating whether the workspace extension supports the specified
        ///     dataset type.
        /// </remarks>
        public virtual bool OwnsDatasetType(esriDatasetType datasetType)
        {
            return false;
        }

        #endregion

        #region IWorkspaceExtensionControl Members

        /// <summary>
        ///     Initializes the extension, passing in a reference to its workspace helper.
        /// </summary>
        /// <param name="pWorkspaceHelper">The workspace helper.</param>
        public virtual void Init(IWorkspaceHelper pWorkspaceHelper)
        {
            _WorkspaceHelper = pWorkspaceHelper;
        }

        /// <summary>
        ///     Informs the extension that its workspace helper (and workspace) are going away.
        /// </summary>
        public virtual void Shutdown()
        {
            _WorkspaceHelper = null;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            GeodatabaseWorkspaceExtensions.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            GeodatabaseWorkspaceExtensions.Unregister(registryKey);
        }

        #endregion
    }
}