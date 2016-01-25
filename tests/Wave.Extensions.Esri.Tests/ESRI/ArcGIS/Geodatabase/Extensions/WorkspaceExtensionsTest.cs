using System;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class WorkspaceExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetDomain_IsNotNull()
        {
            var domains = base.Workspace.GetDomains();
            Assert.IsNotNull(domains);

            var domain = base.Workspace.GetDomain(domains.First().Name);
            Assert.IsNotNull(domain);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetFeatureClass_IsNotNull()
        {
            var table = base.Workspace.GetFeatureClass("", "TRANSFORMER");
            Assert.IsNotNull(table);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetFeatureClasses_Equals_193()
        {
            var list = base.Workspace.GetFeatureClasses();
            Assert.IsNotNull(list);

            var o = list.ToList();
            Assert.AreEqual(193, o.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetFormattedDate_IsNotNull()
        {
            DateTime dateTime = new DateTime(2015, 03, 04);
            var formattedDate = base.Workspace.GetFormattedDate(dateTime);
            Assert.IsNotNull(formattedDate);

            switch (base.Workspace.GetDBMS())
            {
                case DBMS.Access:
                    Assert.AreEqual("#3/4/2015#", formattedDate);
                    break;

                case DBMS.File:
                    Assert.AreEqual("date '3/4/2015'", formattedDate);
                    break;

                case DBMS.Oracle:
                    Assert.AreEqual("04-FEB-15", formattedDate);
                    break;

                case DBMS.SqlServer:
                    Assert.AreEqual("3/14/2015", formattedDate);
                    break;
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetFunctionName_IsNotNull()
        {
            var functionName = base.Workspace.GetFunctionName(esriSQLFunctionName.esriSQL_UPPER);
            Assert.IsNotNull(functionName);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetRelationshipClasses_Equals_153()
        {
            var list = base.Workspace.GetRelationshipClasses();
            Assert.IsNotNull(list);

            var o = list.ToList();
            Assert.AreEqual(153, o.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetRelationship_IsNotNull()
        {
            var table = base.Workspace.GetRelationshipClass("", "DynamicProtDev_RecloserUnit");
            Assert.IsNotNull(table);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetTable_IsNotNull()
        {
            var table = base.Workspace.GetTable("", "ASSEMBLY");
            Assert.IsNotNull(table);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_GetTables_Equals_149()
        {
            var list = base.Workspace.GetTables();
            Assert.IsNotNull(list);

            var o = list.ToList();
            Assert.AreEqual(149, o.Count);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_IsDBMS_IsTrue()
        {
            var dbms = base.Workspace.GetDBMS();
            Assert.IsTrue(base.Workspace.IsDBMS(dbms));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_IsPredicateSupported_IsTrue()
        {
            if (base.Workspace.IsDBMS(DBMS.Access))
            {
                Assert.IsTrue(base.Workspace.IsPredicateSupported(esriSQLPredicates.esriSQL_BETWEEN));
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_PerformOperation_Action_NotNull()
        {
            base.Workspace.PerformOperation(() => true, true, esriMultiuserEditSessionMode.esriMESMNonVersioned);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IWorkspace_StartEditing_NotNull()
        {
            using (var editableWorkspace = base.Workspace.StartEditing(true, esriMultiuserEditSessionMode.esriMESMNonVersioned))
            {
                Assert.IsTrue(editableWorkspace.IsBeingEdited);
                Assert.IsTrue(editableWorkspace.IsInEditOperation);

                editableWorkspace.StopEditing(true);

                Assert.IsFalse(editableWorkspace.IsBeingEdited);
                Assert.IsFalse(editableWorkspace.IsInEditOperation);
            }
        }

        #endregion
    }
}