using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.esriSystem.BaseClasses;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     An helper class used to handle initialization of the ESRI product licenses.
    /// </summary>
    public class EsriRuntimeAuthorization : BaseRuntimeAuthorization<esriLicenseProductCode, esriLicenseExtensionCode, esriLicenseStatus>
    {
        #region Fields

        private readonly IAoInitialize _AoInit;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EsriRuntimeAuthorization" /> class.
        /// </summary>
        public EsriRuntimeAuthorization()
        {
            _AoInit = new AoInitializeClass();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Check in extension when it is no longer needed.
        /// </summary>
        /// <param name="extensionCode">The extension code.</param>
        /// <returns>
        ///     Returns the <see cref="esriLicenseStatus" /> representing the status of the extension.
        /// </returns>
        protected override esriLicenseStatus CheckInExtension(esriLicenseExtensionCode extensionCode)
        {
            return _AoInit.CheckInExtension(extensionCode);
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
                if (_AoInit != null)
                {
                    _AoInit.Shutdown();
                }

                AOUninitialize.Shutdown();
            }
        }

        /// <summary>
        ///     Initializes or (checks out) the extension that correspond to the specified extension code.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <param name="extensionCode">The extension code.</param>
        /// <returns>
        ///     Returns the <see cref="esriLicenseStatus" /> representing the status of the extension.
        /// </returns>
        protected override esriLicenseStatus InitializeExtension(esriLicenseProductCode productCode, esriLicenseExtensionCode extensionCode)
        {
            esriLicenseStatus extensionStatus = _AoInit.IsExtensionCodeAvailable(productCode, extensionCode);
            if (extensionStatus == esriLicenseStatus.esriLicenseAvailable)
                extensionStatus = _AoInit.CheckOutExtension(extensionCode);

            return extensionStatus;
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
            esriLicenseStatus status = _AoInit.IsProductCodeAvailable(productCode);
            if (status == esriLicenseStatus.esriLicenseAvailable)
            {
                status = _AoInit.Initialize(productCode);
                if (IsLicenseInitialized(status))
                {
                    InitializedProduct = _AoInit.InitializedProduct();
                }
            }

            return status;
        }

        /// <summary>
        ///     Determines whether the product has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsLicenseInitialized(esriLicenseStatus licenseStatus)
        {
            return (licenseStatus == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                    licenseStatus == esriLicenseStatus.esriLicenseCheckedOut);
        }

        #endregion
    }
}