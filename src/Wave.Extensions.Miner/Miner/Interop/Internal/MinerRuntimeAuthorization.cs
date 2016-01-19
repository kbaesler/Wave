using ESRI.ArcGIS.esriSystem.BaseClasses;

namespace Miner.Interop.Internal
{
    /// <summary>
    ///     A helper class used to handle initialize the ArcFM licenses.
    /// </summary>
    internal class MinerRuntimeAuthorization : BaseRuntimeAuthorization<mmLicensedProductCode, mmLicensedExtensionCode, mmLicenseStatus>
    {
        #region Fields

        private IMMAppInitialize _AppInit;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the product code that has been initialized.
        /// </summary>
        public override mmLicensedProductCode InitializedProduct
        {
            get { return this.License.InitializedProduct(); }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the license.
        /// </summary>
        /// <value>
        ///     The license.
        /// </value>
        protected IMMAppInitialize License
        {
            get { return _AppInit ?? (_AppInit = new MMAppInitializeClass()); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Check in extension when it is no longer needed.
        /// </summary>
        /// <param name="licensedExtension">The licensed extension.</param>
        /// <returns>
        ///     Returns the <see cref="mmLicenseStatus" /> representing the status of the extension.
        /// </returns>
        public override mmLicenseStatus CheckInExtension(mmLicensedExtensionCode licensedExtension)
        {
            if (this.License.IsExtensionCheckedOut(licensedExtension))
                return this.License.CheckInExtension(licensedExtension);

            return mmLicenseStatus.mmLicenseCheckedIn;
        }

        /// <summary>
        ///     Initializes or (checks out) the extension that correspond to the specified extension code.
        /// </summary>
        /// <param name="licensedProduct">The product code.</param>
        /// <param name="licensedExtension">The extension code.</param>
        /// <returns>
        ///     Returns the <see cref="mmLicenseStatus" /> representing the status of the extension.
        /// </returns>
        public override mmLicenseStatus CheckOutExtension(mmLicensedProductCode licensedProduct, mmLicensedExtensionCode licensedExtension)
        {
            mmLicenseStatus extensionStatus = this.License.IsExtensionCodeAvailable(licensedProduct, licensedExtension);
            if (extensionStatus == mmLicenseStatus.mmLicenseAvailable)
                extensionStatus = this.License.CheckOutExtension(licensedExtension);

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
                if (_AppInit != null)
                {
                    _AppInit.Shutdown();
                }
            }
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
            mmLicenseStatus statusCode = this.License.IsProductCodeAvailable(licensedProduct);
            if (statusCode == mmLicenseStatus.mmLicenseAvailable)
            {
                statusCode = this.License.Initialize(licensedProduct);
                if (this.IsProductInitialized(statusCode))
                {
                    this.InitializedProduct = _AppInit.InitializedProduct();
                }
            }

            return statusCode;
        }

        /// <summary>
        ///     Determines whether the license has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsExtensionInitialized(mmLicenseStatus licenseStatus)
        {
            return (licenseStatus == mmLicenseStatus.mmLicenseAlreadyInitialized ||
                    licenseStatus == mmLicenseStatus.mmLicenseCheckedOut);
        }

        /// <summary>
        ///     Determines whether the product has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsProductInitialized(mmLicenseStatus licenseStatus)
        {
            return (licenseStatus == mmLicenseStatus.mmLicenseAlreadyInitialized ||
                    licenseStatus == mmLicenseStatus.mmLicenseCheckedOut);
        }

        #endregion
    }
}