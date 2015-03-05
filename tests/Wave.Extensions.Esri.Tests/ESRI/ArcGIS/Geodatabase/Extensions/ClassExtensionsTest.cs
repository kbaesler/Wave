using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class ClassExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_Equals_1()
        {
            var testClass = base.GetTestClass();
            var row = testClass.Fetch(1);

            Assert.IsNotNull(row);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_Filter_Action_Equals_6()
        {
            var testClass = base.GetTestClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";

            int rowsAffected = testClass.Fetch(filter, feature => { });

            Assert.AreEqual(6, rowsAffected);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_Filter_Func_Equals_1()
        {
            int testCount = 0;

            var testClass = base.GetTestClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " = 1";

            int rowsAffected = testClass.Fetch(filter, feature =>
            {
                testCount++;

                return true;
            });

            Assert.AreEqual(rowsAffected, testCount);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_Filter_Projection_Equals_6()
        {
            var testClass = base.GetTestClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";

            var list = testClass.Fetch(filter, feature => feature.OID);

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_List_Query_Equals_1()
        {
            var testClass = base.GetTestClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";


            var list = testClass.Fetch(filter);

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_GetSchemaName_IsValid()
        {
            var testClass = base.GetTestClass();
            var schemaName = testClass.GetSchemaName();

            DBMS dbms = base.Workspace.GetDBMS();
            switch (dbms)
            {
                case DBMS.Access:
                case DBMS.File:
                    Assert.IsNull(schemaName);
                    break;

                default:
                    Assert.IsNotNull(schemaName);
                    break;
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_GetSubtypes_Any_IsTrue()
        {
            var testClass = base.GetTestClass();
            var list = testClass.GetSubtypes();
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_GetXDocument_NotNull()
        {
            var testClass = base.GetTestClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " = 1";

            var xdoc = testClass.GetXDocument(filter, field => field.Type == esriFieldType.esriFieldTypeOID);

            Assert.IsNotNull(xdoc);
            Assert.AreEqual(xdoc.Elements().Count(), 1);
        }

        #endregion
    }
}