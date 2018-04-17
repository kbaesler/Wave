using System;
using System.Collections.Generic;
using System.Linq;

namespace ESRI.ArcGIS.esriSystem
{
    /// <summary>
    ///     An interface that provides status information regarding the authorization
    /// </summary>
    public interface IRuntimeAuthorizationStatus
    {
        #region Public Methods

        /// <summary>
        ///     A summary of the status of product and extensions initialization.
        /// </summary>
        /// <returns>Returns a <see cref="string" /> representing the status of the initialization.</returns>
        string GetInitializationStatus();

        #endregion
    }

    /// <summary>
    ///     An internal abstract class used to handle initialization the runtime licenses for use of ArcObjects
    ///     code outside of the ESRI environment.
    /// </summary>
    /// <typeparam name="TLicenseProduct">The type of the product codes.</typeparam>
    /// <typeparam name="TLicenseExtension">The type of the extension codes.</typeparam>
    /// <typeparam name="TLicenseStatus">The type of the status codes.</typeparam>
    public abstract class BaseRuntimeAuthorization<TLicenseProduct, TLicenseExtension, TLicenseStatus> : IDisposable, IRuntimeAuthorizationStatus
    {
        #region Constants

        /// <summary>
        ///     The message extension available
        /// </summary>
        protected const string MESSAGE_EXTENSION_AVAILABLE = "Extension: {0}: Available";

        /// <summary>
        ///     The message extension failed
        /// </summary>
        protected const string MESSAGE_EXTENSION_FAILED = "Extension: {0}: Failed";

        /// <summary>
        ///     The message extension not licensed
        /// </summary>
        protected const string MESSAGE_EXTENSION_NOT_LICENSED = "Extension: {0}: Not Licensed";

        /// <summary>
        ///     The message extension unavailable
        /// </summary>
        protected const string MESSAGE_EXTENSION_UNAVAILABLE = "Extension: {0}: Unavailable";

        /// <summary>
        ///     The message no licenses requested
        /// </summary>
        protected const string MESSAGE_NO_LICENSES_REQUESTED = "Product: No licenses were requested";

        /// <summary>
        ///     The message product available
        /// </summary>
        protected const string MESSAGE_PRODUCT_AVAILABLE = "Product: {0}: Available";

        /// <summary>
        ///     The message product not licensed
        /// </summary>
        protected const string MESSAGE_PRODUCT_NOT_LICENSED = "Product: {0}: Not Licensed";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="BaseRuntimeAuthorization{TLicenseProduct, TLicenseExtension, TLicenseStatus}" /> class.
        /// </summary>
        protected BaseRuntimeAuthorization()
        {
            this.ProductStatus = new Dictionary<TLicenseProduct, TLicenseStatus>();
            this.ExtensionStatus = new Dictionary<TLicenseExtension, TLicenseStatus>();
            this.InitializeLowerProductFirst = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsInitialized
        {
            get
            {
                return this.ProductStatus.All(o => this.IsProductInitialized(o.Value))
                       && this.ExtensionStatus.All(o => this.IsExtensionInitialized(o.Value));
            }
        }

        /// <summary>
        ///     Gets or sets the extension status.
        /// </summary>
        /// <value>
        ///     The extension status.
        /// </value>
        public Dictionary<TLicenseExtension, TLicenseStatus> ExtensionStatus { get; protected set; }

        /// <summary>
        ///     Gets the product code that has been initialized.
        /// </summary>
        public virtual TLicenseProduct InitializedProduct { get; protected set; }

        /// <summary>
        ///     Get/Set the ordering of product code checking. If true, check from lowest to
        ///     highest license. True by default.
        /// </summary>
        public bool InitializeLowerProductFirst { get; set; }

        /// <summary>
        ///     Gets or sets the product status.
        /// </summary>
        /// <value>
        ///     The product status.
        /// </value>
        public Dictionary<TLicenseProduct, TLicenseStatus> ProductStatus { get; protected set; }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IRuntimeAuthorizationStatus Members

        /// <summary>
        ///     A summary of the status of product and extensions initialization.
        /// </summary>
        /// <returns>Returns a <see cref="string" /> representing the status of the initialization.</returns>
        public abstract string GetInitializationStatus();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Check in extension when it is no longer needed.
        /// </summary>
        /// <param name="licenseExtension">The license extension.</param>
        /// <returns>
        ///     Returns the status code representing the status of the extension.
        /// </returns>
        public abstract TLicenseStatus CheckInExtension(TLicenseExtension licenseExtension);

        /// <summary>
        ///     Initializes or (checks out) the extension that correspond to the specified extension code.
        /// </summary>
        /// <param name="licenseProduct">The product code.</param>
        /// <param name="licenseExtension">The extension code.</param>
        /// <returns>
        ///     Returns the status code representing the status of the extension.
        /// </returns>
        public abstract TLicenseStatus CheckOutExtension(TLicenseProduct licenseProduct, TLicenseExtension licenseExtension);

        /// <summary>
        ///     Initialize the application with the specified product code.
        /// </summary>
        /// <param name="licenseProduct">The product codes.</param>
        /// <returns>
        ///     <c>true</c> when the initialization is successful; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     productCodes
        ///     or
        ///     extensionCodes
        /// </exception>
        /// <remarks>
        ///     Make sure an active ArcGIS runtime has been bound before license initialization.
        /// </remarks>
        public bool Initialize(TLicenseProduct licenseProduct)
        {
            return this.Initialize(new[] {licenseProduct}, new TLicenseExtension[] { });
        }

        /// <summary>
        ///     Initialize the application with the specified product and extension codes.
        /// </summary>
        /// <param name="licenseProduct">The product codes.</param>
        /// <param name="licenseExtensions">The license extensions.</param>
        /// <returns>
        ///     <c>true</c> when the initialization is successful; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     productCodes
        ///     or
        ///     extensionCodes
        /// </exception>
        /// <remarks>
        ///     Make sure an active ArcGIS runtime has been bound before license initialization.
        /// </remarks>
        public bool Initialize(TLicenseProduct licenseProduct, params TLicenseExtension[] licenseExtensions)
        {
            return this.Initialize(new[] {licenseProduct}, licenseExtensions);
        }

        /// <summary>
        ///     Initialize the application with the specified product and extension license codes.
        /// </summary>
        /// <param name="licenseProducts">The product codes.</param>
        /// <param name="licenseExtensions">The extension codes.</param>
        /// <returns>
        ///     <c>true</c> when the initialization is successful; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     productCodes
        ///     or
        ///     extensionCodes
        /// </exception>
        /// <remarks>
        ///     Make sure an active ArcGIS runtime has been bound before license initialization.
        /// </remarks>
        public bool Initialize(IEnumerable<TLicenseProduct> licenseProducts, TLicenseExtension[] licenseExtensions)
        {
            bool initializedProduct = false;

            if (licenseProducts == null) throw new ArgumentNullException("licenseProducts");
            if (licenseExtensions == null) throw new ArgumentNullException("licenseExtensions");

            this.ProductStatus.Clear();
            this.ExtensionStatus.Clear();

            // Enumerate through all of the product codes, initializing the product that is available.
            foreach (TLicenseProduct licenseProduct in licenseProducts)
            {
                // Initialize (or check out) the product.
                TLicenseStatus licenseStatus = this.InitializeProduct(licenseProduct);
                this.ProductStatus.Add(licenseProduct, licenseStatus);

                // Verify the product has been initialized.
                if (this.IsProductInitialized(licenseStatus))
                {
                    initializedProduct = true;

                    // Enumerate through all of the extension codes, initializing the extension that is available.
                    foreach (TLicenseExtension licenseExtension in licenseExtensions)
                    {
                        // Initialize (or check out) the extension.
                        licenseStatus = this.CheckOutExtension(licenseProduct, licenseExtension);
                        this.ExtensionStatus.Add(licenseExtension, licenseStatus);

                        // Verify the extension has been initialized.
                        initializedProduct = (initializedProduct && this.IsExtensionInitialized(licenseStatus));
                    }
                }
            }

            return initializedProduct;
        }

        /// <summary>
        ///     Shuts down object and check back in extensions to ensure
        ///     any libraries that have been used are unloaded in the correct order.
        /// </summary>
        /// <remarks>
        ///     Once Shutdown has been called, you cannot re-initialize the product license
        ///     and should not make any ArcObjects call.
        /// </remarks>
        public void Shutdown()
        {
            if (this.ProductStatus.Count > 0 || this.ExtensionStatus.Count > 0)
            {
                foreach (KeyValuePair<TLicenseExtension, TLicenseStatus> item in this.ExtensionStatus)
                    this.CheckInExtension(item.Key);

                this.Dispose(true);

                this.ExtensionStatus.Clear();
                this.ProductStatus.Clear();
            }
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
        protected abstract void Dispose(bool disposing);

        /// <summary>
        ///     Initializes or (checks out) the product licenses that correspond to the specified product code.
        /// </summary>
        /// <param name="licenseProduct">The product code.</param>
        /// <returns>
        ///     Returns the status code representing the status of the product.
        /// </returns>
        protected abstract TLicenseStatus InitializeProduct(TLicenseProduct licenseProduct);

        /// <summary>
        ///     Determines whether the license has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsExtensionInitialized(TLicenseStatus licenseStatus);

        /// <summary>
        ///     Determines whether the license has been initialized based on the status.
        /// </summary>
        /// <param name="licenseStatus">The license status.</param>
        /// <returns>
        ///     <c>true</c> if the license has been initialized based on the status; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsProductInitialized(TLicenseStatus licenseStatus);

        #endregion
    }
}