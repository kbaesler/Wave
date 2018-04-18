using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Framework;

namespace Miner.Interop
{
    /// <summary>
    ///     Abstract base class for Display Namer Objects.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseDisplayNamer : IMMDisplayNamer
    {
        private static readonly ILog Log = LogProvider.For<BaseDisplayNamer>();

        #region Fields

        private readonly string _Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDisplayNamer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseDisplayNamer(string name)
        {
            _Name = name;
        }

        #endregion

        #region IMMDisplayNamer Members

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        ///     The method that is used to determine the display.
        /// </summary>
        /// <param name="pRow">The row.</param>
        /// <returns>
        ///     A string representing the object.
        /// </returns>
        /// <remarks>
        ///     The default implementation is to return the primary display field value for the given row.
        /// </remarks>
        public string DisplayString(IRow pRow)
        {
            try
            {
                return this.InternalExecute(pRow);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    MessageBox.Show(Document.ParentWindow, e.Message, string.Format("Error Executing Display Namer {0}", this.Name), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Error("Error Executing Display Namer " + this.Name, e);
            }

            return "<Error>";
        }

        /// <summary>
        ///     Gets whether the <see cref="BaseDisplayNamer" /> is the enabled.
        /// </summary>
        /// <param name="pDataset">The dataset.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        public bool get_Enabled(IDataset pDataset)
        {
            try
            {
                return this.InternalEnabled(pDataset);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    MessageBox.Show(Document.ParentWindow, e.Message, string.Format(@"Error Enabling Display Namer {0}", this.Name), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Error("Error Enabling Display Namer " + this.Name, e);
            }

            return false;
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
            MMDisplayNameObjects.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MMDisplayNameObjects.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Implementation of DisplayNamer Enabled method for derived classes.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <returns>
        ///     <c>true</c> if the DisplayNamer should be enabled; otherwise <c>false</c>
        /// </returns>
        /// <remarks>
        ///     This method will be called from <see cref="get_Enabled(IDataset)" />
        ///     method
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract bool InternalEnabled(IDataset dataSet);

        /// <summary>
        ///     Implementation of DisplayNamer Execute method for derived classes.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        ///     The string value that will be used as the display name.
        /// </returns>
        /// <remarks>
        ///     This method will be called from <see cref="DisplayString(IRow)" /> method
        ///     and is wrapped within the exception handling for that method.
        /// </remarks>
        protected abstract string InternalExecute(IRow row);

        #endregion
    }
}