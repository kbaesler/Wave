using System;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;

using Miner.Interop;

namespace Wave.Sample.Licenses
{
    /// <summary>
    /// An sample program that demostrates the usage of the <see cref="EsriRuntimeAuthorization"/> and <see cref="RuntimeAuthorization"/> classes.
    /// </summary>
    internal class Program
    {
        #region Public Methods

        public void Run(string[] args)
        {
            Console.WriteLine("=====================");
            Console.WriteLine("Wave Sample: Licenses");
            Console.WriteLine("=====================");

            // ESRI
            this.CheckoutLicenses(esriLicenseProductCode.esriLicenseProductCodeStandard);

            // ARCFM
            this.CheckoutLicenses(esriLicenseProductCode.esriLicenseProductCodeStandard, mmLicensedProductCode.mmLPArcFM);            
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Checkouts the licenses.
        /// </summary>
        /// <param name="esriLicenseProductCode">The esri license product code.</param>
        private void CheckoutLicenses(esriLicenseProductCode esriLicenseProductCode)
        {
            using (var lic = new EsriRuntimeAuthorization())
            {
                if (lic.Initialize(esriLicenseProductCode))
                {
                    Console.WriteLine("Successfully checked-out the {0} license.", esriLicenseProductCode);
                }
                else
                {
                    Console.WriteLine("Unable to check-out the {0} license.", esriLicenseProductCode);
                }
            }            
        }

        /// <summary>
        ///     Checkouts the licenses.
        /// </summary>
        /// <param name="esriLicenseProductCode">The esri license product code.</param>
        /// <param name="mmLicensedProductCode">The mm licensed product code.</param>
        private void CheckoutLicenses(esriLicenseProductCode esriLicenseProductCode, mmLicensedProductCode mmLicensedProductCode)
        {
            using (var lic = new RuntimeAuthorization(ProductCode.Desktop))
            {
                if (lic.Initialize(esriLicenseProductCode, mmLicensedProductCode))
                {
                    Console.WriteLine("Successfully checked-out the {0} and {1} licenses.", esriLicenseProductCode, mmLicensedProductCode);
                }
                else
                {
                    Console.WriteLine("Unable to check-out the {0} and/or {1} license.", esriLicenseProductCode, mmLicensedProductCode);
                }
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                new Program().Run(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }

        #endregion
    }
}