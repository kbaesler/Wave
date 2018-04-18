using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.ComCategories;
using Miner.Framework;

namespace Miner.Interop
{
    /// <summary>
    ///     An abstract namer that supports providing the unique name the files for the map production export.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseNamer : IMMNamer
    {
        private static readonly System.Diagnostics.ILog Log = LogProvider.For<BaseNamer>();

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseNamer" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseNamer(string name)
        {
            this.Name = name;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the feature class.
        /// </summary>
        /// <value>
        ///     The feature class.
        /// </value>
        protected IFeatureClass FeatureClass { get; set; }

        #endregion

        #region IMMNamer Members

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        /// <remarks>
        ///     If the object returns true from SupportsFeatureClass, then this property is used to display in the export naming
        ///     combo box.
        /// </remarks>
        public string Name { get; protected set; }

        /// <summary>
        ///     Gets or sets the name properties.
        /// </summary>
        /// <value>
        ///     The name properties.
        /// </value>
        /// <remarks>
        ///     Allows you to pass in any objects to be used.
        /// </remarks>
        public IPropertySet NamerProperties { get; protected set; }

        /// <summary>
        ///     Called for each plot and should return a unique name for the file to be exported from Map Production.
        ///     The arguments are supplied to provide the developer with a wide range of naming possibilities.
        /// </summary>
        /// <param name="plotExtent">The plot extent.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="tileNumber">The tile number.</param>
        /// <param name="sheetID">The sheet ID.</param>
        /// <param name="baseFileName">Name of the base file.</param>
        /// <param name="baseFileExtension">The base file extension.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the unique name for the file.
        /// </returns>
        public string NextFileName(IGeometry plotExtent, int pageNumber, int tileNumber, string sheetID, string baseFileName, string baseFileExtension)
        {
            try
            {
                return this.GetNextFileName(plotExtent, pageNumber, tileNumber, sheetID, baseFileName, baseFileExtension);
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    MessageBox.Show(Document.ParentWindow, e.Message, string.Format(@"Error Executing File Namer {0}", this.Name), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Error("Error Executing File Namer " + this.Name, e);
            }

            return string.Empty;
        }

        /// <summary>
        ///     Initializes the namer with the specified <paramref name="featureClass" /> and <paramref name="namerProperties" />
        /// </summary>
        /// <param name="featureClass">The feature class.</param>
        /// <param name="namerProperties">The namer properties.</param>
        /// <remarks>
        ///     This method initializes the instance using the data included in the PropertySet by the current map book.
        ///     This is called immediately before NextFileName is called for the first time.
        /// </remarks>
        public virtual void Initialize(IFeatureClass featureClass, IPropertySet namerProperties)
        {
            this.FeatureClass = featureClass;
            this.NamerProperties = namerProperties;
        }

        /// <summary>
        ///     For the selected map set feature layer, the object should return true if it can name export files for this feature
        ///     class.
        /// </summary>
        /// <param name="pFeatureClass">The feature class.</param>
        /// <returns>
        ///     Returns <c>true</c> when the name can export files for this feature class
        /// </returns>
        public virtual bool SupportsFeatureClass(IFeatureClass pFeatureClass)
        {
            return (pFeatureClass != null);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the class using the specified <paramref name="regKey" />.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        internal static void Register(string regKey)
        {
            MMMapProductionNamers.Register(regKey);
        }

        /// <summary>
        ///     Unregisters the class using the specified <paramref name="regKey" />.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string regKey)
        {
            MMMapProductionNamers.Unregister(regKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Called for each plot and should return a unique name for the file to be exported from Map Production.
        ///     The arguments are supplied to provide the developer with a wide range of naming possibilities.
        /// </summary>
        /// <param name="plotExtent">The plot extent.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="tileNumber">The tile number.</param>
        /// <param name="sheetID">The sheet ID.</param>
        /// <param name="baseFileName">Name of the base file.</param>
        /// <param name="baseFileExtension">The base file extension.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the unique name for the file.
        /// </returns>
        protected abstract string GetNextFileName(IGeometry plotExtent, int pageNumber, int tileNumber, string sheetID, string baseFileName, string baseFileExtension);

        #endregion
    }
}