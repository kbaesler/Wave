using System;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class RowExtensionTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void IRow_BlockReentrancy_InvalidOperationException()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            feature.BlockReentrancy();
            feature.SaveChanges();
        }

        [TestMethod]
        public void IRow_Update_PendingChanges_IsFalse()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            var editable = testClass.Fields.AsEnumerable().First(field => field.Editable);
            object value = feature.GetValue(editable.Name, editable.DefaultValue);

            bool pendingUpdates = feature.Update(feature.Fields.Field[1].Name, value);
            Assert.IsTrue(pendingUpdates);
        }

        #endregion
    }
}