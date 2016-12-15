using System.Linq;
using System.Text;

using ESRI.ArcGIS.esriSystem;

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

        /// <summary>
        ///     A summary of the status of product and extensions initialization.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the status of the initialization.
        /// </returns>
        public override string GetInitializationStatus()
        {
            StringBuilder msg = new StringBuilder();

            if (!base.ProductStatus.Any())
            {
                msg.Append(MESSAGE_NO_LICENSES_REQUESTED);
            }
            else if (base.ProductStatus.ContainsValue(mmLicenseStatus.mmLicenseAlreadyInitialized)
                     || base.ProductStatus.ContainsValue(mmLicenseStatus.mmLicenseCheckedOut))
            {
                var status = this.GetProductStatus(this.InitializedProduct, mmLicenseStatus.mmLicenseCheckedOut);
                msg.Append(status);
            }
            else
            {
                foreach (var item in base.ProductStatus)
                {
                    var status = this.GetProductStatus(item.Key, item.Value);
                    msg.AppendLine(status);
                }
            }

            foreach (var item in base.ExtensionStatus)
            {
                var status = this.GetExtensionStatus(item.Key, item.Value);
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
            mmLicenseStatus status = this.License.IsProductCodeAvailable(licensedProduct);
            if (status == mmLicenseStatus.mmLicenseAvailable)
            {
                status = this.License.Initialize(licensedProduct);
                if (this.IsProductInitialized(status))
                {
                    this.InitializedProduct = _AppInit.InitializedProduct();
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

        #region Private Methods

        /// <summary>
        ///     Gets the product status.
        /// </summary>
        /// <param name="licensedExtensionCode">The licensed extension code.</param>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the status of the initialization.
        /// </returns>
        private string GetExtensionStatus(mmLicensedExtensionCode licensedExtensionCode, mmLicenseStatus licenseStatus)
        {
            string extensionName = licensedExtensionCode.ToString();

            switch (licenseStatus)
            {
                case mmLicenseStatus.mmLicenseAlreadyInitialized:
                case mmLicenseStatus.mmLicenseCheckedOut:
                    return string.Format(MESSAGE_EXTENSION_AVAILABLE, extensionName);

                case mmLicenseStatus.mmLicenseCheckedIn:
                    return null;

                case mmLicenseStatus.mmLicenseUnavailable:
                    return string.Format(MESSAGE_EXTENSION_UNAVAILABLE, extensionName);

                case mmLicenseStatus.mmLicenseFailure:
                    return string.Format(MESSAGE_EXTENSION_FAILED, extensionName);

                default:
                    return string.Format(MESSAGE_EXTENSION_NOT_LICENSED, extensionName);
            }
        }

        /// <summary>
        ///     Gets the extension status.
        /// </summary>
        /// <param name="licensedProductCode">The licensed product code.</param>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the status of the initialization.
        /// </returns>
        private string GetProductStatus(mmLicensedProductCode licensedProductCode, mmLicenseStatus licenseStatus)
        {
            string productName = licensedProductCode.ToString();

            switch (licenseStatus)
            {
                case mmLicenseStatus.mmLicenseAlreadyInitialized:
                case mmLicenseStatus.mmLicenseCheckedOut:
                    return string.Format(MESSAGE_PRODUCT_AVAILABLE, productName);

                default:
                    return string.Format(MESSAGE_PRODUCT_NOT_LICENSED, productName);
            }
        }

        #endregion
    }
}