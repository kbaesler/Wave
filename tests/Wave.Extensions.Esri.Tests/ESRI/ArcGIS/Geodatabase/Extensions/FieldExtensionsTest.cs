using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class FieldExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void IFields_AsEnumerable_Count_Equals()
        {
            var testClass = base.GetTestClass();
            var count = testClass.Fields.AsEnumerable().Count();
            Assert.AreEqual(testClass.Fields.FieldCount, count);
        }

        [TestMethod]
        public void IFields_ToDictionary_Count_Equals()
        {
            var testClass = base.GetTestClass();
            var dictionary = testClass.Fields.ToDictionary();

            Assert.AreEqual(testClass.Fields.FieldCount, dictionary.Count);
        }

        #endregion
    }
}