using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class RowExtensionTest : RoadwaysTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        [ExpectedException(typeof (InvalidOperationException))]
        public void IRow_BlockReentrancy_InvalidOperationException()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            row.BlockReentrancy();
            row.SaveChanges();
        }        

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_GetChanges_Dictionary_Contains_2()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            row.Update("MEASURE", 1);
            row.Update("DISTANCE", 1000);

            var list = row.GetChanges();
            Assert.IsTrue(list.Values.Contains("MEASURE"));
            Assert.IsTrue(list.Values.Contains("DISTANCE"));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_GetChanges_FieldName_Equals_1()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            row.Update("MEASURE", 1);

            var list = row.GetChanges("MEASURE");
            Assert.IsTrue(list.Count() == 1);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_TryGetValue_FieldName_Equals_False()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);

            bool value;
            bool condition = row.TryGetValue("FIELD_DOES_NOT_EXISTS", false, out value);
            Assert.IsFalse(condition);
            Assert.IsFalse(value);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_TryGetValue_FieldName_Equals_True()
        {
            var testClass = base.GetTable();
            var row = testClass.Fetch(1);

            int value;
            bool condition = row.TryGetValue(testClass.OIDFieldName, -1, out value);
            Assert.IsTrue(condition);
            Assert.AreNotEqual(-1, value);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        [ExpectedException(typeof (IndexOutOfRangeException))]
        public void IRow_Update_FieldIndex_IndexOutOfRangeException_False_Lower()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            row.Update(-1, null, false);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        [ExpectedException(typeof (IndexOutOfRangeException))]
        public void IRow_Update_FieldIndex_IndexOutOfRangeException_False_Upper()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            row.Update(row.Fields.FieldCount, null, false);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_Update_FieldIndex_IsTrue()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            var editable = testTable.Fields.AsEnumerable().First(field => field.Editable && field.Type == esriFieldType.esriFieldTypeString);
            int i = testTable.FindField(editable.Name);

            bool pendingUpdates = row.Update(i, "ABC");
            Assert.IsTrue(pendingUpdates);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_Update_FieldName_DateTime()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            var testField = testTable.Fields.AsEnumerable().FirstOrDefault(field => field.Editable && field.Type == esriFieldType.esriFieldTypeDate);
            Assert.IsNotNull(testField);

            object date = row.GetValue(testField.Name, default(DateTime?));
            Assert.IsFalse(row.Update(testField.Name, date));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        [ExpectedException(typeof (InvalidCastException))]
        public void IRow_Update_FieldName_InvalidCastException()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            var field = testTable.Fields.AsEnumerable().FirstOrDefault(f => f.Editable);
            Assert.IsNotNull(field);

            row.Update(field.Name, new KeyValuePair<int, string>(1, "One"));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        [ExpectedException(typeof (InvalidCastException))]
        public void IRow_Update_FieldName_InvalidCastException_String()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            var testField = testTable.Fields.AsEnumerable().FirstOrDefault(field => field.Editable && field.Type == esriFieldType.esriFieldTypeString);
            Assert.IsNotNull(testField);

            row.Update(testField.Name, new KeyValuePair<int, string>(1, "One"));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_Update_FieldName_IsFalse()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            var editable = testTable.Fields.AsEnumerable().First(field => field.Editable);
            object value = row.GetValue(editable.Name, editable.DefaultValue);

            bool pendingUpdates = row.Update(row.Fields.Field[1].Name, value);
            Assert.IsFalse(pendingUpdates);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IRow_Update_FieldName_IsTrue()
        {
            var testTable = base.GetTable();
            var row = testTable.Fetch(1);
            Assert.IsNotNull(row);

            var editable = testTable.Fields.AsEnumerable().First(field => field.Editable && field.Type == esriFieldType.esriFieldTypeString);

            bool pendingUpdates = row.Update(editable.Name, "ABC");
            Assert.IsTrue(pendingUpdates);
        }

        #endregion
    }
}