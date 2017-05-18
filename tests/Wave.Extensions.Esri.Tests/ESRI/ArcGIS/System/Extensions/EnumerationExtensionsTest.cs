using ESRI.ArcGIS.esriSystem;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class EnumerationExtensionsTest : RoadwaysTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void esriUnits_ConvertTo()
        {
            var value = esriUnits.esriMiles.ConvertTo(1, esriUnits.esriFeet);
            Assert.AreEqual(5280, value);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void esriUnits_ToLower()
        {
            var value = esriUnits.esriMiles.ToLower(false);
            Assert.AreEqual("mile", value);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void esriUnits_ToLower_Plural()
        {
            var value = esriUnits.esriMiles.ToLower(true);
            Assert.AreEqual("miles", value);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void esriUnits_ToUpper()
        {
            var value = esriUnits.esriMiles.ToUpper(false);
            Assert.AreEqual("MILE", value);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void esriUnits_ToUpper_Plural()
        {
            var value = esriUnits.esriMiles.ToUpper(true);
            Assert.AreEqual("MILES", value);
        }

        #endregion
    }
}