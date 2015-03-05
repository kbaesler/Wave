using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class TableExtensionsTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetClassModelNames_Any_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetClassModelNames();
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetFieldIndex_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldModelNames();
            if (list.Any())
            {
                var index = testClass.GetFieldIndex(list.First().Value.First());
                Assert.IsTrue(index > -1);
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingFieldModelNameException))]
        public void ITable_GetFieldIndex_MissingFieldModelNameException()
        {
            var testClass = base.GetTestTable();
            var index = testClass.GetFieldIndex("");
            Assert.IsTrue(index > -1);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetFieldIndexes_Any_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldIndexes("LOCATABLEFIELD");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetFieldManager_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var fieldManager = testClass.GetFieldManager(0);
            Assert.IsNotNull(fieldManager);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetFieldModelNames_Any_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldModelNames();
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetFieldName_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldModelNames();
            if (list.Any())
            {
                var fieldName = testClass.GetFieldName(list.First().Value.First());
                Assert.IsNotNull(fieldName);
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetFieldNames_Any_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldModelNames();
            if (list.Any())
            {
                var l = testClass.GetFieldNames(list.First().Value.First());
                Assert.IsTrue(l.Any());
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetField_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldModelNames();
            Assert.IsNotNull(list);

            var first = list.FirstOrDefault();
            Assert.IsNotNull(first);

            IField field = testClass.GetField(first.Value.First());
            Assert.IsNotNull(field);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetFields_Any_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldModelNames();
            Assert.IsNotNull(list);

            var first = list.FirstOrDefault();
            Assert.IsNotNull(first);

            var fields = testClass.GetFields(first.Value.ToArray());
            Assert.IsTrue(fields.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetPrimaryDisplayField_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var field = testClass.GetPrimaryDisplayField();
            Assert.IsNotNull(field);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetRelationshipClass_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var relClass = testClass.GetRelationshipClass(esriRelRole.esriRelRoleDestination, "STRUCTURE");
            Assert.IsNotNull(relClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void ITable_GetRelationshipClass_MissingClassModelNameException()
        {
            var testClass = base.GetTestTable();
            var relClass = testClass.GetRelationshipClass(esriRelRole.esriRelRoleOrigin, "STRUCTURE");
            Assert.IsNotNull(relClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetRelationshipClasses_Any_IsFalse()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetRelationshipClasses(esriRelRole.esriRelRoleOrigin, "STRUCTURE");
            Assert.IsFalse(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_GetRelationshipClasses_Any_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetRelationshipClasses(esriRelRole.esriRelRoleDestination, "STRUCTURE");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_IsAssignedClassModelName_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetClassModelNames();
            Assert.IsTrue(testClass.IsAssignedClassModelName(list.First()));
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ITable_IsAssignedFieldModelName_IsTrue()
        {
            var testClass = base.GetTestTable();
            var list = testClass.GetFieldModelNames();
            Assert.IsTrue(testClass.IsAssignedFieldModelName(list.First().Value.First()));
        }

        #endregion
    }
}