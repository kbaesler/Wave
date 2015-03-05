using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class WorkspaceExtensionsTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void IWorkspace_GetFeatureClass_IsNotNull()
        {
            var testClass = base.Workspace.GetFeatureClass("TRANSFORMER");
            Assert.IsNotNull(testClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IWorkspace_GetFeatureClass_MissingClassModelNameException()
        {
            var testClass = base.Workspace.GetFeatureClass("");
            Assert.IsNotNull(testClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IWorkspace_GetFeatureClasses_Any_IsTrue()
        {
            var list = base.Workspace.GetFeatureClasses("DESIGNBANK");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IWorkspace_GetObjectClass_IsNotNull()
        {
            var testClass = base.Workspace.GetObjectClass("TRANSFORMER");
            Assert.IsNotNull(testClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IWorkspace_GetObjectClass_MissingClassModelNameException()
        {
            var testClass = base.Workspace.GetObjectClass("");
            Assert.IsNotNull(testClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IWorkspace_GetObjectClasses_Any_IsTrue()
        {
            var list = base.Workspace.GetObjectClasses("DESIGNBANK");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IWorkspace_GetTable_IsNotNull()
        {
            var testClass = base.Workspace.GetTable("TRANSFORMERUNIT");
            Assert.IsNotNull(testClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IWorkspace_GetTable_MissingClassModelNameException()
        {
            var testClass = base.Workspace.GetTable("");
            Assert.IsNotNull(testClass);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IWorkspace_GetTables_Any_IsTrue()
        {
            var list = base.Workspace.GetTables("DESIGNUNIT");
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IWorkspace_IsAssignedDatabaseModelName_Any_IsTrue()
        {
            Assert.IsTrue(base.Workspace.IsAssignedDatabaseModelName("MM ENTERPRISE"));
        }

        #endregion
    }
}