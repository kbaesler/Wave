using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop;

namespace Wave.Extensions.Miner.Tests.Miner.Interop
{
    [TestClass]
    public class RuntimeAuthorizationTests
    {
        [TestMethod]
        public void RuntimeAuthorization_GetInitializationStatus()
        {
            using (var lic = new RuntimeAuthorization())
            {
                var extensionCodes = Enum.GetValues(typeof(mmLicensedExtensionCode));
                lic.Initialize(mmLicensedProductCode.mmLPArcFM, extensionCodes.OfType<mmLicensedExtensionCode>().ToArray());

                var status = lic.GetInitializationStatus();
                Assert.IsNotNull(status);
            }
        }
    }
}
