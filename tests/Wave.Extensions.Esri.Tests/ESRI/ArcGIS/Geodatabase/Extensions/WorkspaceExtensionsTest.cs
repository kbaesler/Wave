using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class WorkspaceExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void IWorkspace_EditableWorkspace_NotNull()
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

        [TestMethod]
        public void IWorkspace_IsDBMS_IsTrue()
        {
            var dbms = base.Workspace.GetDBMS();
            Assert.IsTrue(base.Workspace.IsDBMS(dbms));
        }

        [TestMethod]
        public void IWorkspace_IsPredicateSupported_IsTrue()
        {
            if (base.Workspace.IsDBMS(DBMS.Access))
            {
                Assert.IsTrue(base.Workspace.IsPredicateSupported(esriSQLPredicates.esriSQL_BETWEEN));
            }
        }

        #endregion
    }
}