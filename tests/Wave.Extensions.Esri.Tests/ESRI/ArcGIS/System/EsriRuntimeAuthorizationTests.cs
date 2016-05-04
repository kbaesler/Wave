using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.esriSystem;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class EsriRuntimeAuthorizationTests 
    {
        [TestMethod]
        public void GetInitializationStatus()
        {
            using (var lic = new EsriRuntimeAuthorization())
            {
                var status = lic.GetInitializationStatus();
                Assert.AreEqual("Product: No licenses were requested\r\n", status);

                var extensionCodes = Enum.GetValues(typeof (esriLicenseExtensionCode));
                lic.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard, extensionCodes.OfType<esriLicenseExtensionCode>().ToArray());

                status = lic.GetInitializationStatus();
                Assert.IsNotNull(status);
            }            
        }
    }
}
