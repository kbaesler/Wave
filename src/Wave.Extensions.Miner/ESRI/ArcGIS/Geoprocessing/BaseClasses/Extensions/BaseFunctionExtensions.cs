using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    /// Provides extension methods for the <see cref="BaseFunction"/> class.
    /// </summary>
    public static class BaseFunctionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Verifies that the workspace is at a valid version for ArcFM.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="messages">The messagesfor the geoproccessing tool.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the workspace is the correct version.
        /// </returns>
        public static bool VerifyVersion(this BaseFunction source, IWorkspace workspace, IGPMessages messages)
        {
            IMMArcFMDatabaseUpgradeInfo upgradeInfo = new MMArcFMDatabaseUpgradeInfoClass();
            upgradeInfo.Workspace = workspace;

            if (upgradeInfo.UpgradeRequired)
            {
                messages.AddWarning(string.Format("Please run the Upgrade ArcFM Solution Database tool before modifying the database. An upgrade from {0} to {1} is required.", upgradeInfo.CurrentBuildNumber, upgradeInfo.UpgradeBuildNumber));
                return false;
            }

            return true;
        }

        #endregion
    }
}