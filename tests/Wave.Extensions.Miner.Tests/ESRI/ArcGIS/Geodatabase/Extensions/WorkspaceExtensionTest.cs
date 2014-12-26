using System;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;
using Miner.Interop;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class WorkspaceExtensionTest : ProcessTests
    {
        public WorkspaceExtensionTest()
            : base(mmProductInstallation.mmPIArcFM)
        {
            
        }

        [TestMethod]
        public void IWorkspace_CreateAdoConnection()
        {
            var c = base.Workspace.CreateAdoConnection();
            Assert.IsNotNull(c);
            Assert.AreEqual(base.PxApplication.Connection, c);
        }
    }
}
