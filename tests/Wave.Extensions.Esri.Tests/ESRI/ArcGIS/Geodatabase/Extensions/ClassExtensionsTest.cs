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
        public void IFeatureClass_CreateNew_IsNotNull()
        {
            var testClass = base.GetTestClass();
            var feature = testClass.CreateNew();
            Assert.IsNotNull(feature);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateQuery_EndsWith_Character()
        {
            var testClass = base.GetTestClass();
            var filter = testClass.CreateExpression("kellyl", ComparisonOperator.EndsWith, LogicalOperator.Or);

            Assert.IsNotNull(filter);
            Assert.AreEqual("(UPPER(CREATIONUSER) Like '%KELLYL') Or (CAST(DATECREATED As CHAR(11)) Like '%kellyl') Or (CAST(DATEMODIFIED As CHAR(12)) Like '%kellyl') Or (UPPER(LASTUSER) Like '%KELLYL') Or (UPPER(FACILITYID) Like '%KELLYL') Or (UPPER(FEEDERID) Like '%KELLYL') Or (UPPER(FEEDERID2) Like '%KELLYL') Or (UPPER(COMMENTS) Like '%KELLYL') Or (UPPER(WORKORDERID) Like '%KELLYL') Or (CAST(INSTALLATIONDATE As CHAR(16)) Like '%kellyl') Or (UPPER(HIGHSIDEPROTECTION) Like '%KELLYL') Or (UPPER(LOCATIONTYPE) Like '%KELLYL') Or (UPPER(LABELTEXT) Like '%KELLYL') Or (UPPER(HIGHSIDECONFIGURATION) Like '%KELLYL') Or (UPPER(LOWSIDECONFIGURATION) Like '%KELLYL') Or (UPPER(LOADTAPCHANGERINDICATOR) Like '%KELLYL') Or (UPPER(LOWSIDEPROTECTION) Like '%KELLYL') Or (UPPER(SWITCHTYPE) Like '%KELLYL') Or (UPPER(TERTIARYCONFIGURATION) Like '%KELLYL') Or (UPPER(WORKREQUESTID) Like '%KELLYL') Or (UPPER(DESIGNID) Like '%KELLYL') Or (UPPER(WORKLOCATIONID) Like '%KELLYL') Or (UPPER(GlobalID) Like '%KELLYL') Or (UPPER(FilledWeight) Like '%KELLYL') Or (UPPER(EmptyWeight) Like '%KELLYL') Or (UPPER(HeightBushings) Like '%KELLYL') Or (UPPER(HeightNoBushings) Like '%KELLYL') Or (UPPER(AlternateX) Like '%KELLYL') Or (UPPER(AlternateY) Like '%KELLYL') Or (UPPER(AlternateZ) Like '%KELLYL') Or (UPPER(AlternateSource) Like '%KELLYL')", filter.WhereClause);

            int rowCount = testClass.FeatureCount(filter);
            Assert.IsTrue(rowCount > 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateQuery_EndsWith_Numeric()
        {
            var testClass = base.GetTestClass();
            var filter = testClass.CreateExpression("6", ComparisonOperator.EndsWith, LogicalOperator.Or);

            Assert.IsNotNull(filter);
            Assert.AreEqual("(CAST(OBJECTID As CHAR(8)) Like '%6') Or (CAST(ANCILLARYROLE As CHAR(13)) Like '%6') Or (CAST(ENABLED As CHAR(7)) Like '%6') Or (UPPER(CREATIONUSER) Like '%6') Or (CAST(DATECREATED As CHAR(11)) Like '%6') Or (CAST(DATEMODIFIED As CHAR(12)) Like '%6') Or (UPPER(LASTUSER) Like '%6') Or (CAST(SUBTYPECD As CHAR(9)) Like '%6') Or (UPPER(FACILITYID) Like '%6') Or (UPPER(FEEDERID) Like '%6') Or (UPPER(FEEDERID2) Like '%6') Or (CAST(OPERATINGVOLTAGE As CHAR(16)) Like '%6') Or (UPPER(COMMENTS) Like '%6') Or (UPPER(WORKORDERID) Like '%6') Or (CAST(INSTALLATIONDATE As CHAR(16)) Like '%6') Or (CAST(ELECTRICTRACEWEIGHT As CHAR(19)) Like '%6') Or (CAST(FEEDERINFO As CHAR(10)) Like '%6') Or (CAST(SYMBOLROTATION As CHAR(14)) Like '%6') Or (CAST(GROUNDREACTANCE As CHAR(15)) Like '%6') Or (CAST(GROUNDRESISTANCE As CHAR(16)) Like '%6') Or (CAST(HIGHSIDEGROUNDREACTANCE As CHAR(23)) Like '%6') Or (CAST(HIGHSIDEGROUNDRESISTANCE As CHAR(24)) Like '%6') Or (UPPER(HIGHSIDEPROTECTION) Like '%6') Or (UPPER(LOCATIONTYPE) Like '%6') Or (CAST(MAGNETIZINGREACTANCE As CHAR(20)) Like '%6') Or (CAST(MAGNETIZINGRESISTANCE As CHAR(21)) Like '%6') Or (UPPER(LABELTEXT) Like '%6') Or (CAST(PHASEDESIGNATION As CHAR(16)) Like '%6') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%6') Or (CAST(RATEDKVA As CHAR(8)) Like '%6') Or (UPPER(HIGHSIDECONFIGURATION) Like '%6') Or (UPPER(LOWSIDECONFIGURATION) Like '%6') Or (UPPER(LOADTAPCHANGERINDICATOR) Like '%6') Or (CAST(LOWSIDEGROUNDREACTANCE As CHAR(22)) Like '%6') Or (CAST(LOWSIDEGROUNDRESISTANCE As CHAR(23)) Like '%6') Or (UPPER(LOWSIDEPROTECTION) Like '%6') Or (CAST(LOWSIDEVOLTAGE As CHAR(14)) Like '%6') Or (CAST(RATEDKVA65RISE As CHAR(14)) Like '%6') Or (CAST(RATEDTERTIARYKVA As CHAR(16)) Like '%6') Or (UPPER(SWITCHTYPE) Like '%6') Or (UPPER(TERTIARYCONFIGURATION) Like '%6') Or (CAST(TERTIARYVOLTAGE As CHAR(15)) Like '%6') Or (UPPER(WORKREQUESTID) Like '%6') Or (UPPER(DESIGNID) Like '%6') Or (UPPER(WORKLOCATIONID) Like '%6') Or (CAST(WORKFLOWSTATUS As CHAR(14)) Like '%6') Or (CAST(WORKFUNCTION As CHAR(12)) Like '%6') Or (UPPER(GlobalID) Like '%6') Or (CAST(ParentCircuitSourceID As CHAR(21)) Like '%6') Or (CAST(CircuitSourceID As CHAR(15)) Like '%6') Or (CAST(SubSource As CHAR(9)) Like '%6') Or (UPPER(FilledWeight) Like '%6') Or (UPPER(EmptyWeight) Like '%6') Or (UPPER(HeightBushings) Like '%6') Or (UPPER(HeightNoBushings) Like '%6') Or (UPPER(AlternateX) Like '%6') Or (UPPER(AlternateY) Like '%6') Or (UPPER(AlternateZ) Like '%6') Or (UPPER(AlternateSource) Like '%6')", filter.WhereClause);

            int rowCount = testClass.FeatureCount(filter);
            Assert.IsTrue(rowCount > 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateQuery_Like_Character()
        {
            var testClass = base.GetTestClass();
            var filter = testClass.CreateExpression("kellyl", ComparisonOperator.Contains, LogicalOperator.Or);

            Assert.IsNotNull(filter);
            Assert.AreEqual("(UPPER(CREATIONUSER) Like '%KELLYL%') Or (CAST(DATECREATED As CHAR(11)) Like '%kellyl%') Or (CAST(DATEMODIFIED As CHAR(12)) Like '%kellyl%') Or (UPPER(LASTUSER) Like '%KELLYL%') Or (UPPER(FACILITYID) Like '%KELLYL%') Or (UPPER(FEEDERID) Like '%KELLYL%') Or (UPPER(FEEDERID2) Like '%KELLYL%') Or (UPPER(COMMENTS) Like '%KELLYL%') Or (UPPER(WORKORDERID) Like '%KELLYL%') Or (CAST(INSTALLATIONDATE As CHAR(16)) Like '%kellyl%') Or (UPPER(HIGHSIDEPROTECTION) Like '%KELLYL%') Or (UPPER(LOCATIONTYPE) Like '%KELLYL%') Or (UPPER(LABELTEXT) Like '%KELLYL%') Or (UPPER(HIGHSIDECONFIGURATION) Like '%KELLYL%') Or (UPPER(LOWSIDECONFIGURATION) Like '%KELLYL%') Or (UPPER(LOADTAPCHANGERINDICATOR) Like '%KELLYL%') Or (UPPER(LOWSIDEPROTECTION) Like '%KELLYL%') Or (UPPER(SWITCHTYPE) Like '%KELLYL%') Or (UPPER(TERTIARYCONFIGURATION) Like '%KELLYL%') Or (UPPER(WORKREQUESTID) Like '%KELLYL%') Or (UPPER(DESIGNID) Like '%KELLYL%') Or (UPPER(WORKLOCATIONID) Like '%KELLYL%') Or (UPPER(GlobalID) Like '%KELLYL%') Or (UPPER(FilledWeight) Like '%KELLYL%') Or (UPPER(EmptyWeight) Like '%KELLYL%') Or (UPPER(HeightBushings) Like '%KELLYL%') Or (UPPER(HeightNoBushings) Like '%KELLYL%') Or (UPPER(AlternateX) Like '%KELLYL%') Or (UPPER(AlternateY) Like '%KELLYL%') Or (UPPER(AlternateZ) Like '%KELLYL%') Or (UPPER(AlternateSource) Like '%KELLYL%')", filter.WhereClause);

            int rowCount = testClass.FeatureCount(filter);
            Assert.IsTrue(rowCount > 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateQuery_Like_Numeric()
        {
            var testClass = base.GetTestClass();
            var filter = testClass.CreateExpression("6", ComparisonOperator.Contains, LogicalOperator.Or);

            Assert.IsNotNull(filter);
            Assert.AreEqual("(CAST(OBJECTID As CHAR(8)) Like '%6%') Or (CAST(ANCILLARYROLE As CHAR(13)) Like '%6%') Or (CAST(ENABLED As CHAR(7)) Like '%6%') Or (UPPER(CREATIONUSER) Like '%6%') Or (CAST(DATECREATED As CHAR(11)) Like '%6%') Or (CAST(DATEMODIFIED As CHAR(12)) Like '%6%') Or (UPPER(LASTUSER) Like '%6%') Or (CAST(SUBTYPECD As CHAR(9)) Like '%6%') Or (UPPER(FACILITYID) Like '%6%') Or (UPPER(FEEDERID) Like '%6%') Or (UPPER(FEEDERID2) Like '%6%') Or (CAST(OPERATINGVOLTAGE As CHAR(16)) Like '%120%') Or (CAST(OPERATINGVOLTAGE As CHAR(16)) Like '%110%') Or (UPPER(COMMENTS) Like '%6%') Or (UPPER(WORKORDERID) Like '%6%') Or (CAST(INSTALLATIONDATE As CHAR(16)) Like '%6%') Or (CAST(ELECTRICTRACEWEIGHT As CHAR(19)) Like '%6%') Or (CAST(FEEDERINFO As CHAR(10)) Like '%6%') Or (CAST(SYMBOLROTATION As CHAR(14)) Like '%6%') Or (CAST(GROUNDREACTANCE As CHAR(15)) Like '%6%') Or (CAST(GROUNDRESISTANCE As CHAR(16)) Like '%6%') Or (CAST(HIGHSIDEGROUNDREACTANCE As CHAR(23)) Like '%6%') Or (CAST(HIGHSIDEGROUNDRESISTANCE As CHAR(24)) Like '%6%') Or (UPPER(HIGHSIDEPROTECTION) Like '%6%') Or (UPPER(LOCATIONTYPE) Like '%6%') Or (CAST(MAGNETIZINGREACTANCE As CHAR(20)) Like '%6%') Or (CAST(MAGNETIZINGRESISTANCE As CHAR(21)) Like '%6%') Or (UPPER(LABELTEXT) Like '%6%') Or (CAST(PHASEDESIGNATION As CHAR(16)) Like '%6%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%120%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%110%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%420%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%440%') Or (CAST(RATEDKVA As CHAR(8)) Like '%6%') Or (UPPER(HIGHSIDECONFIGURATION) Like '%6%') Or (UPPER(LOWSIDECONFIGURATION) Like '%6%') Or (UPPER(LOADTAPCHANGERINDICATOR) Like '%6%') Or (CAST(LOWSIDEGROUNDREACTANCE As CHAR(22)) Like '%6%') Or (CAST(LOWSIDEGROUNDRESISTANCE As CHAR(23)) Like '%6%') Or (UPPER(LOWSIDEPROTECTION) Like '%6%') Or (CAST(LOWSIDEVOLTAGE As CHAR(14)) Like '%160%') Or (CAST(LOWSIDEVOLTAGE As CHAR(14)) Like '%230%') Or (CAST(LOWSIDEVOLTAGE As CHAR(14)) Like '%270%') Or (CAST(RATEDKVA65RISE As CHAR(14)) Like '%6%') Or (CAST(RATEDTERTIARYKVA As CHAR(16)) Like '%6%') Or (UPPER(SWITCHTYPE) Like '%6%') Or (UPPER(TERTIARYCONFIGURATION) Like '%6%') Or (CAST(TERTIARYVOLTAGE As CHAR(15)) Like '%6%') Or (UPPER(WORKREQUESTID) Like '%6%') Or (UPPER(DESIGNID) Like '%6%') Or (UPPER(WORKLOCATIONID) Like '%6%') Or (CAST(WORKFLOWSTATUS As CHAR(14)) Like '%6%') Or (CAST(WORKFUNCTION As CHAR(12)) Like '%6%') Or (UPPER(GlobalID) Like '%6%') Or (CAST(ParentCircuitSourceID As CHAR(21)) Like '%6%') Or (CAST(CircuitSourceID As CHAR(15)) Like '%6%') Or (CAST(SubSource As CHAR(9)) Like '%6%') Or (UPPER(FilledWeight) Like '%6%') Or (UPPER(EmptyWeight) Like '%6%') Or (UPPER(HeightBushings) Like '%6%') Or (UPPER(HeightNoBushings) Like '%6%') Or (UPPER(AlternateX) Like '%6%') Or (UPPER(AlternateY) Like '%6%') Or (UPPER(AlternateZ) Like '%6%') Or (UPPER(AlternateSource) Like '%6%')", filter.WhereClause);

            int rowCount = testClass.FeatureCount(filter);
            Assert.IsTrue(rowCount > 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateQuery_StartsWith_Character()
        {
            var testClass = base.GetTestClass();
            var filter = testClass.CreateExpression("kellyl", ComparisonOperator.StartsWith, LogicalOperator.Or);

            Assert.IsNotNull(filter);
            Assert.AreEqual("(UPPER(CREATIONUSER) Like 'KELLYL%') Or (CAST(DATECREATED As CHAR(11)) Like 'kellyl%') Or (CAST(DATEMODIFIED As CHAR(12)) Like 'kellyl%') Or (UPPER(LASTUSER) Like 'KELLYL%') Or (UPPER(FACILITYID) Like 'KELLYL%') Or (UPPER(FEEDERID) Like 'KELLYL%') Or (UPPER(FEEDERID2) Like 'KELLYL%') Or (UPPER(COMMENTS) Like 'KELLYL%') Or (UPPER(WORKORDERID) Like 'KELLYL%') Or (CAST(INSTALLATIONDATE As CHAR(16)) Like 'kellyl%') Or (UPPER(HIGHSIDEPROTECTION) Like 'KELLYL%') Or (UPPER(LOCATIONTYPE) Like 'KELLYL%') Or (UPPER(LABELTEXT) Like 'KELLYL%') Or (UPPER(HIGHSIDECONFIGURATION) Like 'KELLYL%') Or (UPPER(LOWSIDECONFIGURATION) Like 'KELLYL%') Or (UPPER(LOADTAPCHANGERINDICATOR) Like 'KELLYL%') Or (UPPER(LOWSIDEPROTECTION) Like 'KELLYL%') Or (UPPER(SWITCHTYPE) Like 'KELLYL%') Or (UPPER(TERTIARYCONFIGURATION) Like 'KELLYL%') Or (UPPER(WORKREQUESTID) Like 'KELLYL%') Or (UPPER(DESIGNID) Like 'KELLYL%') Or (UPPER(WORKLOCATIONID) Like 'KELLYL%') Or (UPPER(GlobalID) Like 'KELLYL%') Or (UPPER(FilledWeight) Like 'KELLYL%') Or (UPPER(EmptyWeight) Like 'KELLYL%') Or (UPPER(HeightBushings) Like 'KELLYL%') Or (UPPER(HeightNoBushings) Like 'KELLYL%') Or (UPPER(AlternateX) Like 'KELLYL%') Or (UPPER(AlternateY) Like 'KELLYL%') Or (UPPER(AlternateZ) Like 'KELLYL%') Or (UPPER(AlternateSource) Like 'KELLYL%')", filter.WhereClause);

            int rowCount = testClass.FeatureCount(filter);
            Assert.IsTrue(rowCount > 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureClass_CreateQuery_StartsWith_Numeric()
        {
            var testClass = base.GetTestClass();
            var filter = testClass.CreateExpression("6", ComparisonOperator.StartsWith, LogicalOperator.Or);

            Assert.IsNotNull(filter);
            Assert.AreEqual("(CAST(OBJECTID As CHAR(8)) Like '%6%') Or (CAST(ANCILLARYROLE As CHAR(13)) Like '%6%') Or (CAST(ENABLED As CHAR(7)) Like '%6%') Or (UPPER(CREATIONUSER) Like '%6%') Or (CAST(DATECREATED As CHAR(11)) Like '%6%') Or (CAST(DATEMODIFIED As CHAR(12)) Like '%6%') Or (UPPER(LASTUSER) Like '%6%') Or (CAST(SUBTYPECD As CHAR(9)) Like '%6%') Or (UPPER(FACILITYID) Like '%6%') Or (UPPER(FEEDERID) Like '%6%') Or (UPPER(FEEDERID2) Like '%6%') Or (CAST(OPERATINGVOLTAGE As CHAR(16)) Like '%120%') Or (CAST(OPERATINGVOLTAGE As CHAR(16)) Like '%110%') Or (UPPER(COMMENTS) Like '%6%') Or (UPPER(WORKORDERID) Like '%6%') Or (CAST(INSTALLATIONDATE As CHAR(16)) Like '%6%') Or (CAST(ELECTRICTRACEWEIGHT As CHAR(19)) Like '%6%') Or (CAST(FEEDERINFO As CHAR(10)) Like '%6%') Or (CAST(SYMBOLROTATION As CHAR(14)) Like '%6%') Or (CAST(GROUNDREACTANCE As CHAR(15)) Like '%6%') Or (CAST(GROUNDRESISTANCE As CHAR(16)) Like '%6%') Or (CAST(HIGHSIDEGROUNDREACTANCE As CHAR(23)) Like '%6%') Or (CAST(HIGHSIDEGROUNDRESISTANCE As CHAR(24)) Like '%6%') Or (UPPER(HIGHSIDEPROTECTION) Like '%6%') Or (UPPER(LOCATIONTYPE) Like '%6%') Or (CAST(MAGNETIZINGREACTANCE As CHAR(20)) Like '%6%') Or (CAST(MAGNETIZINGRESISTANCE As CHAR(21)) Like '%6%') Or (UPPER(LABELTEXT) Like '%6%') Or (CAST(PHASEDESIGNATION As CHAR(16)) Like '%6%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%120%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%110%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%420%') Or (CAST(NOMINALVOLTAGE As CHAR(14)) Like '%440%') Or (CAST(RATEDKVA As CHAR(8)) Like '%6%') Or (UPPER(HIGHSIDECONFIGURATION) Like '%6%') Or (UPPER(LOWSIDECONFIGURATION) Like '%6%') Or (UPPER(LOADTAPCHANGERINDICATOR) Like '%6%') Or (CAST(LOWSIDEGROUNDREACTANCE As CHAR(22)) Like '%6%') Or (CAST(LOWSIDEGROUNDRESISTANCE As CHAR(23)) Like '%6%') Or (UPPER(LOWSIDEPROTECTION) Like '%6%') Or (CAST(LOWSIDEVOLTAGE As CHAR(14)) Like '%160%') Or (CAST(LOWSIDEVOLTAGE As CHAR(14)) Like '%230%') Or (CAST(LOWSIDEVOLTAGE As CHAR(14)) Like '%270%') Or (CAST(RATEDKVA65RISE As CHAR(14)) Like '%6%') Or (CAST(RATEDTERTIARYKVA As CHAR(16)) Like '%6%') Or (UPPER(SWITCHTYPE) Like '%6%') Or (UPPER(TERTIARYCONFIGURATION) Like '%6%') Or (CAST(TERTIARYVOLTAGE As CHAR(15)) Like '%6%') Or (UPPER(WORKREQUESTID) Like '%6%') Or (UPPER(DESIGNID) Like '%6%') Or (UPPER(WORKLOCATIONID) Like '%6%') Or (CAST(WORKFLOWSTATUS As CHAR(14)) Like '%6%') Or (CAST(WORKFUNCTION As CHAR(12)) Like '%6%') Or (UPPER(GlobalID) Like '%6%') Or (CAST(ParentCircuitSourceID As CHAR(21)) Like '%6%') Or (CAST(CircuitSourceID As CHAR(15)) Like '%6%') Or (CAST(SubSource As CHAR(9)) Like '%6%') Or (UPPER(FilledWeight) Like '%6%') Or (UPPER(EmptyWeight) Like '%6%') Or (UPPER(HeightBushings) Like '%6%') Or (UPPER(HeightNoBushings) Like '%6%') Or (UPPER(AlternateX) Like '%6%') Or (UPPER(AlternateY) Like '%6%') Or (UPPER(AlternateZ) Like '%6%') Or (UPPER(AlternateSource) Like '%6%')", filter.WhereClause);

            int rowCount = testClass.FeatureCount(filter);
            Assert.IsTrue(rowCount > 0);
        }

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
            }, true);

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