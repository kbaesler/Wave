using System.Collections.Generic;

using ESRI.ArcGIS.GeoDatabaseUI;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides access to members that control the change version dialog for a versioned geodatabase.
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.GeoDatabaseUI.IChangeVersionDialog" />
    public interface IChangeVersionDialog2 : IChangeVersionDialog
    {
        #region Public Methods

        /// <summary>
        /// Displays the dialog used to create new versions in a versioned geodatabase.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="versions">The versions.</param>
        /// <returns></returns>
        bool DoModal(IWorkspace workspace, IEnumerable<IVersionInfo> versions);

        #endregion
    }
}