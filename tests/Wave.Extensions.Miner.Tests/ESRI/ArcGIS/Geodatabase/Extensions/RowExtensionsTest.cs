using System;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class RowExtensionTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        [ExpectedException(typeof (IndexOutOfRangeException))]
        public void IRow_GetValue_FieldModelName_IndexOutOfRangeException()
        {
            IFeatureClass testClass = base.GetTestClass();
            IFeature feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            object value = feature.GetValue("@%4123%1", string.Empty, false);
            Assert.AreEqual(string.Empty, value);
        }

        [TestMethod]
        public void IRow_GetValue_FieldModelName_IsNotNull()
        {
            IFeatureClass testClass = base.GetTestClass();
            IFeature feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            object value = feature.GetValue("FEEDERID", string.Empty, false);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        [ExpectedException(typeof (MissingFieldModelNameException))]
        public void IRow_GetValue_FieldModelName_MissingFieldModelNameException()
        {
            IFeatureClass testClass = base.GetTestClass();
            IFeature feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            object value = feature.GetValue("@%4123%1", string.Empty, true);
            Assert.IsNotNull(value);
        }

        #endregion
    }
}