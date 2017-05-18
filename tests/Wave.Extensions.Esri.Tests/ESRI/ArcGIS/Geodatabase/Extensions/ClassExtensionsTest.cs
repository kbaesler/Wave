using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class ClassExtensionsTest : RoadwaysTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateExpression_EndsWith_Character()
        {
            var testClass = base.GetTable();
            var expression = testClass.CreateExpression("10002101", ComparisonOperator.EndsWith, LogicalOperator.Or);

            Assert.IsNotNull(expression);
            Assert.AreEqual("(CAST(OBJECTID As CHAR(8)) Like '%10002101') Or (UPPER(ROUTEID) Like '%10002101') Or (CAST(MEASURE As CHAR(7)) Like '%10002101') Or (CAST(DISTANCE As CHAR(8)) Like '%10002101') Or (UPPER(CASE_NUMBER) Like '%10002101') Or (CAST(CASE_YEAR As CHAR(9)) Like '%10002101') Or (UPPER(REFERENCE_MARKER) Like '%10002101') Or (UPPER(ROAD_SYSTEM) Like '%10002101') Or (CAST(NUMBER_FATALITIES As CHAR(17)) Like '%10002101') Or (CAST(NUMBER_INJURIES As CHAR(15)) Like '%10002101') Or (UPPER(REPORTABLE) Like '%10002101') Or (UPPER(POLICE_DEPARTMENT) Like '%10002101') Or (UPPER(INTERSECTION_NUMBER) Like '%10002101') Or (UPPER(MUNICIPALITY) Like '%10002101') Or (CAST(NUMBER_VEHICLES As CHAR(15)) Like '%10002101') Or (UPPER(ACCIDENT_TYPE) Like '%10002101') Or (UPPER(LOCATION) Like '%10002101') Or (UPPER(TRAFFIC_CONTROL) Like '%10002101') Or (UPPER(LIGHT_CONDITION) Like '%10002101') Or (UPPER(WEATHER) Like '%10002101') Or (UPPER(ROAD_CHARACTER) Like '%10002101') Or (UPPER(ROAD_SURFACE_CONDITION) Like '%10002101') Or (UPPER(COLLISION_TYPE) Like '%10002101') Or (UPPER(PEDESTRIAN_LOCATION) Like '%10002101') Or (UPPER(PEDESTRIAN_ACTION) Like '%10002101') Or (UPPER(EXTENT_OF_INJURIES) Like '%10002101') Or (UPPER(REGION_COUNTY) Like '%10002101') Or (UPPER(LOW_NODE) Like '%10002101') Or (UPPER(HIGH_NODE) Like '%10002101') Or (UPPER(REPORTING_AGENCY) Like '%10002101') Or (UPPER(OFFICER_BUILDING_NUMBER) Like '%10002101') Or (UPPER(DMV_ACCIDENT_CLASSIFICATION) Like '%10002101') Or (UPPER(LOCATION_ERROR_CODE) Like '%10002101') Or (UPPER(COMM_VEH_ACC_IND) Like '%10002101') Or (UPPER(HIGHWAY_IND) Like '%10002101') Or (UPPER(INTERSECTION_IND) Like '%10002101') Or (CAST(UTM_NORTHING As CHAR(12)) Like '%10002101') Or (CAST(UTM_EASTING As CHAR(11)) Like '%10002101') Or (UPPER(REL_ACC_VEH) Like '%10002101') Or (CAST(CRASH_SEVERITY As CHAR(14)) Like '%10002101') Or (CAST(TEST_DOMAIN As CHAR(11)) Like '%10002101') Or (UPPER(EVENT_ID) Like '%10002101')", expression);

            int rowCount = testClass.RowCount(new QueryFilterClass() {WhereClause = expression});
            Assert.IsTrue(rowCount > 0);
        }
        
        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateNew_IsNotNull()
        {
            var testClass = base.GetLineFeatureClass();

            Assert.IsFalse(base.Workspace.PerformOperation(true, esriMultiuserEditSessionMode.esriMESMVersioned, () =>
            {
                var feature = testClass.CreateNew();
                Assert.IsNotNull(feature);
                return false;
            }));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_Equals_1()
        {
            var testClass = base.GetPointFeatureClass();
            var row = testClass.Fetch(1);

            Assert.IsNotNull(row);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_Filter_Action_Equals_6()
        {
            var testClass = base.GetPointFeatureClass();

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

            var testClass = base.GetLineFeatureClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " = 1";

            int rowsAffected = testClass.Fetch(filter, feature =>
            {
                testCount++;

                return true;
            }, true);

            Assert.AreEqual(rowsAffected, testCount);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_Filter_Projection_Equals_6()
        {
            var testClass = base.GetPointFeatureClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";

            var list = testClass.Fetch(filter, feature => feature.OID);

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_Fetch_List_Query_Equals_1()
        {
            var testClass = base.GetPointFeatureClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";


            var list = testClass.Fetch(filter);

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_GetSchemaName_IsValid()
        {
            var testClass = base.GetLineFeatureClass();
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
            var testClass = base.GetLineFeatureClass();
            var list = testClass.GetSubtypes();
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_GetXDocument_NotNull()
        {
            var testClass = base.GetLineFeatureClass();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " = 1";

            var xdoc = testClass.GetXDocument(filter, field => field.Type == esriFieldType.esriFieldTypeOID);

            Assert.IsNotNull(xdoc);
            Assert.AreEqual(xdoc.Elements().Count(), 1);
        }

        #endregion
    }
}