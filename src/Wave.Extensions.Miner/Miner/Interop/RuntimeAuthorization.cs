using System;
using System.Text;

using ESRI.ArcGIS.esriSystem;

using Miner.Interop.Internal;
#if V10
using ESRI.ArcGIS;

#endif

namespace Miner.Interop
{
    /// <summary>
    ///     A supporting class used to check out the licenses necessary to run applications outside of Miner and Miner and ESRI
    ///     products.
    /// </summary>
    public class RuntimeAuthorization : IDisposable, IRuntimeAuthorizationStatus
    {
        #region Fields

        private readonly EsriRuntimeAuthorization _EsriRuntime;
        private readonly MinerRuntimeAuthorization _MinerRuntime;

        #endregion

        #region Constructors

#if !V10
    /// <summary>
    ///     Initializes a new instance of the <see cref="RuntimeAuthorization" /> class.
    /// </summary>
        public RuntimeAuthorization()
        {
            _EsriRuntime = new EsriRuntimeAuthorization();
            _MinerRuntime = new MinerRuntimeAuthorization();            
        }
#else
        /// <summary>
        ///     Initializes a new instance of the <see cref="RuntimeAuthorization" /> class.
        /// </summary>
        public RuntimeAuthorization()
            : this(ProductCode.EngineOrDesktop)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RuntimeAuthorization" /> class.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        public RuntimeAuthorization(ProductCode productCode)
        {
            _EsriRuntime = new EsriRuntimeAuthorization();
            _EsriRuntime.ResolveRuntimeBinding += (sender, args) => args.ProductCode = productCode;
            _MinerRuntime = new MinerRuntimeAuthorization();
        }
#endif

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

        #region Public Methods

        /// <summary>
        ///     Attempts to checkout the license for the specific ESRI <paramref name="licensedProduct" />.
        /// </summary>
        /// <param name="licensedProduct">The licensed product.</param>
        /// <param name="licensedExtension">The licensed extension.</param>
        /// <returns>
        ///     Returns a <see cref="Boolean" /> representing <c>true</c> when the initialization is successful; otherwise
        ///     <c>false</c>.
        /// </returns>
        public bool Initialize(esriLicenseProductCode licensedProduct, params esriLicenseExtensionCode[] licensedExtension)
        {
            return _EsriRuntime.Initialize(new[] {licensedProduct}, licensedExtension);
        }

        /// <summary>
        ///     Attempts to checkout the license for the specific ArcFM <paramref name="licensedProduct" />.
        /// </summary>
        /// <param name="licensedProduct">The licensed product.</param>
        /// <param name="licensedExtension">The extension codes.</param>
        /// <returns>
        ///     Returns a <see cref="Boolean" /> representing <c>true</c> when the initialization is successful; otherwise
        ///     <c>false</c>.
        /// </returns>
        public bool Initialize(mmLicensedProductCode licensedProduct, params mmLicensedExtensionCode[] licensedExtension)
        {
            return _MinerRuntime.Initialize(new[] {licensedProduct}, licensedExtension);
        }

        /// <summary>
        ///     Attempts to checkout the license for the specific ArcFM <paramref name="mmLicensedProduct" /> and ESRI
        ///     <paramref name="esriLicensedProduct" />.
        /// </summary>
        /// <param name="esriLicensedProduct">The ArFM product code.</param>
        /// <param name="mmLicensedProduct">The mm licensed product.</param>
        /// <returns>
        ///     Returns a <see cref="Boolean" /> representing  <c>true</c> when the initialization is successful; otherwise
        ///     <c>false</c>.
        /// </returns>
        public bool Initialize(esriLicenseProductCode esriLicensedProduct, mmLicensedProductCode mmLicensedProduct)
        {
            return this.Initialize(esriLicensedProduct) && this.Initialize(mmLicensedProduct);
        }

        /// <summary>
        ///     A summary of the status of product and extensions initialization.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the status of the initialization.
        /// </returns>
        public string GetInitializationStatus()
        {
            return string.Format("ESRI {0} and ArcFM {1}", _EsriRuntime.GetInitializationStatus(), _MinerRuntime.GetInitializationStatus());
        }

        /// <summary>
        ///     Checks in all ArcGIS and ArcFM licenses that have been checked out.
        /// </summary>
        public void Shutdown()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_MinerRuntime != null)
                    _MinerRuntime.Dispose();

                if (_EsriRuntime != null)
                    _EsriRuntime.Dispose();
            }
        }

        #endregion        
    }
}