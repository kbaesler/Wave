using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class FieldExtensionsTest : RoadwaysTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFields_AsEnumerable_Count_Equals()
        {
            var testClass = base.GetLineFeatureClass();
            var count = testClass.Fields.AsEnumerable().Count();
            Assert.AreEqual(testClass.Fields.FieldCount, count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFields_ToDictionary_Count_Equals()
        {
            var testClass = base.GetLineFeatureClass();
            var dictionary = testClass.Fields.ToDictionary();

            Assert.AreEqual(testClass.Fields.FieldCount, dictionary.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFields_ToDictionary_IgnoreCase_True()
        {
            var testClass = base.GetLineFeatureClass();
            var dictionary = testClass.Fields.ToDictionary();

            Assert.IsTrue(dictionary.ContainsKey(testClass.Fields.Field[0].Name.ToLower()));
        }

        #endregion
    }
}