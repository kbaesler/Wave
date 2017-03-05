using System.Linq;
using System.Text;

using ESRI.ArcGIS.ADF.COMSupport;
#if V10
using System;
using System.Collections.Generic;
#endif

namespace ESRI.ArcGIS.esriSystem
{
#if V10
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

#if V10
        /// <summary>
        ///     Raised when ArcGIS runtime binding hasn't been established.
        /// </summary>
        public event EventHandler<RuntimeManagerEventArgs> ResolveRuntimeBinding;
#endif

        #endregion

        #region Constructors

#if V10
        /// <summary>
        ///     Initializes a new instance of the <see cref="EsriRuntimeAuthorization" /> class.
        /// </summary>
        public EsriRuntimeAuthorization()
            : this(ProductCode.EngineOrDesktop)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EsriRuntimeAuthorization" /> class.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        public EsriRuntimeAuthorization(ProductCode productCode)
        {
            this.ResolveRuntimeBinding += (sender, args) => args.ProductCode = productCode;
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="EsriRuntimeAuthorization"/> class.
        /// </summary>
        public EsriRuntimeAuthorization() { }
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
#if V10
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

        /// <summary>
        ///     A summary of the status of product and extensions initialization.
        /// </summary>
        /// <returns>Returns a <see cref="string" /> representing the status of the initialization.</returns>
        public override string GetInitializationStatus()
        {
            StringBuilder msg = new StringBuilder();

            if (!base.ProductStatus.Any())
            {
                msg.Append(MESSAGE_NO_LICENSES_REQUESTED);
            }
            else if (base.ProductStatus.ContainsValue(esriLicenseStatus.esriLicenseAlreadyInitialized)
                     || base.ProductStatus.ContainsValue(esriLicenseStatus.esriLicenseCheckedOut))
            {
                var status = this.GetProductStatus(this.License as ILicenseInformation, this.InitializedProduct, esriLicenseStatus.esriLicenseCheckedOut);
                msg.Append(status);
            }
            else
            {
                foreach (var item in base.ProductStatus)
                {
                    var status = this.GetProductStatus(this.License as ILicenseInformation, item.Key, item.Value);
                    msg.AppendLine(status);
                }
            }

            foreach (var item in base.ExtensionStatus)
            {
                var status = this.GetExtensionStatus(this.License as ILicenseInformation, item.Key, item.Value);
                msg.AppendLine(status);
            }

            return msg.ToString();
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
        /// <param name="licensedProduct">The product code.</param>
        /// <returns>
        ///     Returns the <see cref="esriLicenseStatus" /> representing the status of the product.
        /// </returns>
        protected override esriLicenseStatus InitializeProduct(esriLicenseProductCode licensedProduct)
        {
            esriLicenseStatus status = this.License.IsProductCodeAvailable(licensedProduct);
            if (status == esriLicenseStatus.esriLicenseAvailable)
            {
                status = this.License.Initialize(licensedProduct);
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

#if V10
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

        #region Private Methods

        /// <summary>
        ///     Gets the product status.
        /// </summary>
        /// <param name="licenseInformation">The license information.</param>
        /// <param name="licenseProductCode">The license product code.</param>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>Returns a <see cref="string" /> representing the status of the initialization.</returns>
        private string GetProductStatus(ILicenseInformation licenseInformation, esriLicenseProductCode licenseProductCode, esriLicenseStatus licenseStatus)
        {
            string productName;

            try
            {
                productName = licenseInformation.GetLicenseProductName(licenseProductCode);
            }
            catch
            {
                productName = licenseProductCode.ToString();
            }

            switch (licenseStatus)
            {
                case esriLicenseStatus.esriLicenseAlreadyInitialized:
                case esriLicenseStatus.esriLicenseCheckedOut:
                    return string.Format(MESSAGE_PRODUCT_AVAILABLE, productName);

                default:
                    return string.Format(MESSAGE_PRODUCT_NOT_LICENSED, productName);
            }
        }

        /// <summary>
        ///     Gets the extension status.
        /// </summary>
        /// <param name="licenseInformation">The license information.</param>
        /// <param name="licenseExtensionCode">The license extension code.</param>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>Returns a <see cref="string" /> representing the status of the initialization.</returns>
        private string GetExtensionStatus(ILicenseInformation licenseInformation, esriLicenseExtensionCode licenseExtensionCode, esriLicenseStatus licenseStatus)
        {
            string extensionName;

            try
            {
                extensionName = licenseInformation.GetLicenseExtensionName(licenseExtensionCode);
            }
            catch
            {
                extensionName = licenseExtensionCode.ToString();
            }

            switch (licenseStatus)
            {
                case esriLicenseStatus.esriLicenseAlreadyInitialized:
                case esriLicenseStatus.esriLicenseCheckedOut:
                    return string.Format(MESSAGE_EXTENSION_AVAILABLE, extensionName);

                case esriLicenseStatus.esriLicenseCheckedIn:
                    return null;

                case esriLicenseStatus.esriLicenseUnavailable:
                    return string.Format(MESSAGE_EXTENSION_UNAVAILABLE, extensionName);

                case esriLicenseStatus.esriLicenseFailure:
                    return string.Format(MESSAGE_EXTENSION_FAILED, extensionName);

                default:
                    return string.Format(MESSAGE_EXTENSION_NOT_LICENSED, extensionName);
            }
        }

        #endregion        
    }
}