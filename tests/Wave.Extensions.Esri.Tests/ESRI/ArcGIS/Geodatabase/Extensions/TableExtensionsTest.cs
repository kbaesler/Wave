using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class TableExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_CreateExpression_Contains_Character()
        {
            var testClass = base.GetTestTable();
            var expression = testClass.CreateExpression("kellyl", ComparisonOperator.Contains, LogicalOperator.Or);

            Assert.IsNotNull(expression);
            Assert.AreEqual("(UPPER(CREATIONUSER) Like '%KELLYL%') Or (UPPER(LASTUSER) Like '%KELLYL%') Or (UPPER(FRAMINGTYPE) Like '%KELLYL%') Or (UPPER(WORKREQUESTID) Like '%KELLYL%') Or (UPPER(DESIGNID) Like '%KELLYL%') Or (UPPER(WORKLOCATIONID) Like '%KELLYL%') Or (UPPER(GlobalID) Like '%KELLYL%')", expression);

            int rowCount = testClass.RowCount(new QueryFilterClass() {WhereClause = expression});
            Assert.IsTrue(rowCount == 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_CreateExpression_EndsWith_Character()
        {
            var testClass = base.GetTestTable();
            var expression = testClass.CreateExpression("kellyl", ComparisonOperator.EndsWith, LogicalOperator.Or);

            Assert.IsNotNull(expression);
            Assert.AreEqual("(UPPER(CREATIONUSER) Like '%KELLYL') Or (UPPER(LASTUSER) Like '%KELLYL') Or (UPPER(FRAMINGTYPE) Like '%KELLYL') Or (UPPER(WORKREQUESTID) Like '%KELLYL') Or (UPPER(DESIGNID) Like '%KELLYL') Or (UPPER(WORKLOCATIONID) Like '%KELLYL') Or (UPPER(GlobalID) Like '%KELLYL')", expression);

            int rowCount = testClass.RowCount(new QueryFilterClass() {WhereClause = expression});
            Assert.IsTrue(rowCount == 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_CreateExpression_StartsWith_Character()
        {
            var testClass = base.GetTestTable();
            var expression = testClass.CreateExpression("kellyl", ComparisonOperator.StartsWith, LogicalOperator.Or);

            Assert.IsNotNull(expression);
            Assert.AreEqual("(UPPER(CREATIONUSER) Like 'KELLYL%') Or (UPPER(LASTUSER) Like 'KELLYL%') Or (UPPER(FRAMINGTYPE) Like 'KELLYL%') Or (UPPER(WORKREQUESTID) Like 'KELLYL%') Or (UPPER(DESIGNID) Like 'KELLYL%') Or (UPPER(WORKLOCATIONID) Like 'KELLYL%') Or (UPPER(GlobalID) Like 'KELLYL%')", expression);

            int rowCount = testClass.RowCount(new QueryFilterClass() {WhereClause = expression});
            Assert.IsTrue(rowCount == 0);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_CreateNew_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var feature = testClass.CreateNew();
            Assert.IsNotNull(feature);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_Fetch_Filter_Action_Equals_6()
        {
            var testClass = base.GetTestTable();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";

            int rowsAffected = testClass.Fetch(filter, feature => { });
            Assert.AreEqual(6, rowsAffected);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_Fetch_Filter_Func_Equals_1()
        {
            var testClass = base.GetTestTable();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";

            int rowsAffected = testClass.Fetch(filter, row => true, true);
            Assert.AreEqual(6, rowsAffected);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_Fetch_Filter_Projection_Equals_6()
        {
            var testClass = base.GetTestTable();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " IN (1,2,3,4,5,6)";

            var list = testClass.Fetch(filter, row => row.OID);

            Assert.AreEqual(6, list.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_Fetch_Row_IsNotNull()
        {
            var testClass = base.GetTestTable();
            var row = testClass.Fetch(1);

            Assert.IsNotNull(row);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ITable_GetXDocument_NotNull()
        {
            var testClass = base.GetTestTable();

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = testClass.OIDFieldName + " = 1";

            var xdoc = testClass.GetXDocument(filter, field => field.Type == esriFieldType.esriFieldTypeOID);

            Assert.IsNotNull(xdoc);
            Assert.AreEqual(xdoc.Elements().Count(), 1);
        }

        #endregion
    }
}