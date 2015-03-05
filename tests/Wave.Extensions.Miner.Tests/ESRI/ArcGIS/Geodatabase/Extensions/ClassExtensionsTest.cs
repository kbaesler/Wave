using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class ClassExtensionsTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetClassModelNames_Any_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetClassModelNames();
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetFieldIndex_IsNotNull()
        {
            var testClass = base.GetTestClass();
            var index = testClass.GetFieldIndex("LOCATABLEFIELD");
            Assert.IsTrue(index > -1);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingFieldModelNameException))]
        public void IFeatureClass_GetFieldIndex_MissingFieldModelNameException()
        {
            var testClass = base.GetTestClass();
            var index = testClass.GetFieldIndex("");
            Assert.IsTrue(index > -1);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetFieldIndexes_Any_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetFieldIndexes("LOCATABLEFIELD");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetFieldManager_IsNotNull()
        {
            var testClass = base.GetTestClass();
            var fieldManager = testClass.GetFieldManager(0);
            Assert.IsNotNull(fieldManager);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetFieldModelNames_Any_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetFieldModelNames();
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetFieldName_IsNotNull()
        {
            var testClass = base.GetTestClass();
            var fieldName = testClass.GetFieldName("LOCATABLEFIELD");
            Assert.IsNotNull(fieldName);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetFieldNames_Any_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetFieldNames("LOCATABLEFIELD");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetField_IsNotNull()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetFieldModelNames();
            Assert.IsNotNull(list);

            var first = list.FirstOrDefault();
            Assert.IsNotNull(first);

            IField field = testClass.GetField(first.Value.First());
            Assert.IsNotNull(field);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetFields_Any_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetFieldModelNames();
            Assert.IsNotNull(list);

            var first = list.FirstOrDefault();
            Assert.IsNotNull(first);

            var fields = testClass.GetFields(first.Value.ToArray());
            Assert.IsTrue(fields.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetPrimaryDisplayField_IsNotNull()
        {
            var testClass = base.GetTestClass();
            var field = testClass.GetPrimaryDisplayField();
            Assert.IsNotNull(field);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetRelationshipClass_IsNotNull()
        {
            var testClass = base.GetTestClass();
            var relClass = testClass.GetRelationshipClass(esriRelRole.esriRelRoleOrigin, "TRANSFORMERUNIT");
            Assert.IsNotNull(relClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IFeatureClass_GetRelationshipClass_MissingClassModelNameException()
        {
            var testClass = base.GetTestClass();
            var relClass = testClass.GetRelationshipClass(esriRelRole.esriRelRoleDestination, "TRANSFORMERUNIT");
            Assert.IsNotNull(relClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetRelationshipClasses_Any_IsFalse()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetRelationshipClasses(esriRelRole.esriRelRoleDestination, "TRANSFORMERUNIT");
            Assert.IsFalse(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_GetRelationshipClasses_Any_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetRelationshipClasses(esriRelRole.esriRelRoleOrigin, "TRANSFORMERUNIT");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_IsAssignedClassModelName_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetClassModelNames();
            Assert.IsTrue(testClass.IsAssignedClassModelName(list.First()));
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IFeatureClass_IsAssignedFieldModelName_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetFieldModelNames();
            Assert.IsTrue(testClass.IsAssignedFieldModelName(list.First().Value.First()));
        }

        #endregion
    }
}