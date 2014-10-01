using ESRI.ArcGIS.BaseClasses;

using Miner.Interop;

namespace Miner.System.Internal
{
    /// <summary>
    ///     A helper class used to handle initialize the ArcFM licenses.
    /// </summary>
    internal class MinerRuntimeAuthorizatio : BaseRuntimeAuthorization<mmLicensedProductCode, mmLicensedExtensionCode, mmLicenseStatus>
    {
        #region Fields

        private readonly IMMAppInitialize _AppInit;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MinerRuntimeAuthorizatio" /> class.
        /// </summary>
        public MinerRuntimeAuthorizatio()
        {
            _AppInit = new MMAppInitializeClass();

#if V_10
            IMMEsriBind bind = new MMEsriBindClass();
            bind.AutoBind();
#endif
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the product code that has been initialized.
        /// </summary>
        public override mmLicensedProductCode InitializedProduct
        {
            get { return _AppInit.InitializedProduct(); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Check in extension when it is no longer needed.
        /// </summary>
        /// <param name="licensedExtension">The licensed extension.</param>
        /// <returns>
        ///     Returns the <see cref="mmLicenseStatus" /> representing the status of the extension.
        /// </returns>
        protected override mmLicenseStatus CheckInExtension(mmLicensedExtensionCode licensedExtension)
        {
            return _AppInit.CheckInExtension(licensedExtension);
        }

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
                if (_AppInit != null)
                {
                    _AppInit.Shutdown();
                }
            }
        }

        /// <summary>
        ///     Initializes or (checks out) the extension that correspond to the specified extension code.
        /// </summary>
        /// <param name="licensedProduct">The product code.</param>
        /// <param name="licensedExtension">The extension code.</param>
        /// <returns>
        ///     Returns the <see cref="mmLicenseStatus" /> representing the status of the extension.
        /// </returns>
        protected override mmLicenseStatus InitializeExtension(mmLicensedProductCode licensedProduct, mmLicensedExtensionCode licensedExtension)
        {
            mmLicenseStatus extensionStatus = _AppInit.IsExtensionCodeAvailable(licensedProduct, licensedExtension);
            if (extensionStatus == mmLicenseStatus.mmLicenseAvailable)
                extensionStatus = _AppInit.CheckOutExtension(licensedExtension);

            return extensionStatus;
        }

        /// <summary>
        ///     Initializes or (checks out) the product licenses that correspond to the specified product code.
        /// </summary>
        /// <param name="licensedProduct">The product code.</param>
        /// <returns>
        ///     Returns the <see cref="mmLicenseStatus" /> representing the status of the product.
        /// </returns>
        protected override mmLicenseStatus InitializeProduct(mmLicensedProductCode licensedProduct)
        {
            mmLicenseStatus statusCode = _AppInit.IsProductCodeAvailable(licensedProduct);
            if (statusCode == mmLicenseStatus.mmLicenseAvailable)
            {
                statusCode = _AppInit.Initialize(licensedProduct);
                if (this.IsLicenseInitialized(statusCode))
                {
                    this.InitializedProduct = _AppInit.InitializedProduct();
                }
            }

            return statusCode;
        }

        /// <summary>
        ///     Determines whether the product has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsLicenseInitialized(mmLicenseStatus licenseStatus)
        {
            return (licenseStatus == mmLicenseStatus.mmLicenseAlreadyInitialized ||
                    licenseStatus == mmLicenseStatus.mmLicenseCheckedOut);
        }

        #endregion
    }
}