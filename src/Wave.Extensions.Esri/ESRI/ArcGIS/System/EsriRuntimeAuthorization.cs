using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.esriSystem.BaseClasses;

namespace ESRI.ArcGIS.esriSystem
{

#if ARC10
    /// <summary>
    ///     The runtime manager event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class RuntimeManagerEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RuntimeManagerEventArgs" /> class.
        /// </summary>
        /// <param name="installedRuntimes">The installed runtimes.</param>
        public RuntimeManagerEventArgs(IEnumerable<RuntimeInfo> installedRuntimes)
        {
            this.InstalledRuntimes = installedRuntimes;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the installed runtimes.
        /// </summary>
        /// <value>
        ///     The installed runtimes.
        /// </value>
        public IEnumerable<RuntimeInfo> InstalledRuntimes { get; private set; }

        /// <summary>
        ///     Gets or sets the product code.
        /// </summary>
        /// <value>
        ///     The product code.
        /// </value>
        public ProductCode ProductCode { get; set; }

        #endregion
    }
#endif

    /// <summary>
    ///     An helper class used to handle initialization of the ESRI product licenses.
    /// </summary>
    public class EsriRuntimeAuthorization : BaseRuntimeAuthorization<esriLicenseProductCode, esriLicenseExtensionCode, esriLicenseStatus>
    {
        #region Fields

        private IAoInitialize _AoInit;

        #endregion

        #region Events

#if ARC10
        /// <summary>
        ///     Raised when ArcGIS runtime binding hasn't been established.
        /// </summary>
        public event EventHandler<RuntimeManagerEventArgs> ResolveRuntimeBinding;
#endif

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the license.
        /// </summary>
        /// <value>
        ///     The license.
        /// </value>
        protected IAoInitialize License
        {
            get
            {
#if ARC10
                if (RuntimeManager.ActiveRuntime == null)
                {
                    var eventArgs = new RuntimeManagerEventArgs(RuntimeManager.InstalledRuntimes);
                    this.OnResolveRuntimeBinding(eventArgs);

                    if (!RuntimeManager.Bind(eventArgs.ProductCode))
                        throw new Exception(string.Format("Product: {0}: Unavailable", eventArgs.ProductCode));
                }
#endif
                return _AoInit ?? (_AoInit = new AoInitializeClass());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Check in extension when it is no longer needed.
        /// </summary>
        /// <param name="extensionCode">The extension code.</param>
        /// <returns>
        ///     Returns the <see cref="esriLicenseStatus" /> representing the status of the extension.
        /// </returns>
        public override esriLicenseStatus CheckInExtension(esriLicenseExtensionCode extensionCode)
        {
            if (this.License.IsExtensionCheckedOut(extensionCode))
                return this.License.CheckInExtension(extensionCode);

            return esriLicenseStatus.esriLicenseCheckedIn;
        }

        /// <summary>
        ///     Initializes or (checks out) the extension that correspond to the specified extension code.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <param name="extensionCode">The extension code.</param>
        /// <returns>
        ///     Returns the <see cref="esriLicenseStatus" /> representing the status of the extension.
        /// </returns>
        public override esriLicenseStatus CheckOutExtension(esriLicenseProductCode productCode, esriLicenseExtensionCode extensionCode)
        {
            esriLicenseStatus extensionStatus = this.License.IsExtensionCodeAvailable(productCode, extensionCode);
            if (extensionStatus == esriLicenseStatus.esriLicenseAvailable)
                extensionStatus = this.License.CheckOutExtension(extensionCode);

            return extensionStatus;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_AoInit != null)
                {
                    _AoInit.Shutdown();
                }

                AOUninitialize.Shutdown();
            }
        }

        /// <summary>
        ///     Initializes or (checks out) the product licenses that correspond to the specified product code.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <returns>
        ///     Returns the <see cref="esriLicenseStatus" /> representing the status of the product.
        /// </returns>
        protected override esriLicenseStatus InitializeProduct(esriLicenseProductCode productCode)
        {
            esriLicenseStatus status = this.License.IsProductCodeAvailable(productCode);
            if (status == esriLicenseStatus.esriLicenseAvailable)
            {
                status = this.License.Initialize(productCode);
                if (this.IsProductInitialized(status))
                {
                    this.InitializedProduct = this.License.InitializedProduct();
                }
            }

            return status;
        }

        /// <summary>
        ///     Determines whether the license has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsExtensionInitialized(esriLicenseStatus licenseStatus)
        {
            return (licenseStatus == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                    licenseStatus == esriLicenseStatus.esriLicenseCheckedOut);
        }

        /// <summary>
        ///     Determines whether the product has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsProductInitialized(esriLicenseStatus licenseStatus)
        {
            return (licenseStatus == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                    licenseStatus == esriLicenseStatus.esriLicenseCheckedOut);
        }

#if ARC10
        /// <summary>
        ///     Raises the <see cref="E:ResolveRuntimeBinding" /> event.
        /// </summary>
        /// <param name="e">The <see cref="RuntimeManagerEventArgs" /> instance containing the event data.</param>
        protected virtual void OnResolveRuntimeBinding(RuntimeManagerEventArgs e)
        {
            var eventHandler = this.ResolveRuntimeBinding;
            if (eventHandler != null)
                eventHandler(this, e);
        }
#endif

        #endregion
    }
}