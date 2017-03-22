using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;

using Miner.Geodatabase;
using Miner.Interop;

namespace Wave.Geoprocessing.Toolbox.Management
{
    /// <summary>
    /// Provides a geoprocesing tool that enables or disables based on the required licensed product.
    ///</summary>
    public abstract class BaseLicensedFunction : BaseFunction
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseLicensedFunction" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="functionFactory">The function factory.</param>
        /// <param name="requiredProduct">The required product.</param>
        protected BaseLicensedFunction(string name, string alias, IGPFunctionFactory functionFactory, mmProductInstallation requiredProduct)
            : base(name, alias, functionFactory)
        {
            this.RequiredProduct = requiredProduct;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the required product.
        /// </summary>
        /// <value>
        ///     The required product.
        /// </value>
        protected mmProductInstallation RequiredProduct { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the geoprocessing function has all necessary licenses in order to execute.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="T:System.Boolean" /> representing <c>true</c> when the function is licensed.
        /// </returns>
        /// <remarks>
        ///     The IsLicensed property is used to check if a function tool is licensed to execute in the active application.
        /// </remarks>
        public override bool IsLicensed()
        {
            IMMLicensedProductManager manager = new MMProductManagerClass();
            return manager.IsInstalledProduct[this.RequiredProduct];
        }

        #endregion      

        #region Protected Methods

        /// <summary>
        ///     Verifies that the <paramref name="workspace" /> is at a valid ArcFM version.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="messages">The messagesfor the geoproccessing tool.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the workspace is the correct version.
        /// </returns>
        protected bool VerifyVersion(IWorkspace workspace, IGPMessages messages)
        {
            IMMArcFMDatabaseUpgradeInfo upgradeInfo = new DatabaseUpgradeInfo(workspace);
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