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
        public void IRow_GetChanges_Dictionary_Contains_2()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            feature.Update("DATEMODIFIED", DateTime.Now);
            feature.Update("LASTUSER", Environment.UserName);

            var list = feature.GetChanges();
            Assert.IsTrue(list.Values.Contains("DATEMODIFIED"));
            Assert.IsTrue(list.Values.Contains("LASTUSER"));
        }

        [TestMethod]
        public void IRow_GetChanges_List_Equals_1()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            feature.Update("DATEMODIFIED", DateTime.Now);
            feature.Update("LASTUSER", Environment.UserName);

            var list = feature.GetChanges("DATEMODIFIED", "OBJECTID");
            Assert.IsTrue(list.Count == 1);
        }

        [TestMethod]
        public void IRow_TryGetValue_Equals_False()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();

            bool value;
            bool condition = feature.TryGetValue("FIELD_DOES_NOT_EXISTS", false, out value);
            Assert.IsFalse(condition);
            Assert.IsFalse(value);
        }

        [TestMethod]
        public void IRow_TryGetValue_Equals_True()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();

            int value;
            bool condition = feature.TryGetValue(testClass.OIDFieldName, -1, out value);
            Assert.IsTrue(condition);
            Assert.AreNotEqual(-1, value);
        }

        [TestMethod]
        [ExpectedException(typeof (IndexOutOfRangeException))]
        public void IRow_Update_IndexOutOfRangeException_Negative()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            feature.Update(-1, null, false);
        }

        [TestMethod]
        [ExpectedException(typeof (IndexOutOfRangeException))]
        public void IRow_Update_IndexOutOfRangeException_Positive()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.Fetch(1).FirstOrDefault();
            Assert.IsNotNull(feature);

            feature.Update(feature.Fields.FieldCount, null, false);
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