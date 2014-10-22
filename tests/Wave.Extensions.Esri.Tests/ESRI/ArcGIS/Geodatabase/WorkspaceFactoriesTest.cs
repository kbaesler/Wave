using System;
using System.IO;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wave.Extensions.Esri.Tests.Properties;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class WorkspaceFactoriesTests
    {
        #region Public Methods

        [TestMethod]
        [ExpectedException(typeof (FileNotFoundException))]
        public void WorkspaceFactories_GetFactory_FileNotFoundException()
        {
            Assert.IsNull(WorkspaceFactories.GetFactory("@C:\\kadhfakjfh.sde"));
        }

        [TestMethod]
        public void WorkspaceFactories_GetFactory_NotNull()
        {
            Assert.IsNotNull(WorkspaceFactories.GetFactory(Settings.Default.TestData));
        }

        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void WorkspaceFactories_GetFactory_NotSupportedException()
        {
            Assert.IsNull(WorkspaceFactories.GetFactory(""));
        }

        [TestMethod]
        [ExpectedException(typeof (FileNotFoundException))]
        public void WorkspaceFactories_Open_FileNotFoundException()
        {
            Assert.IsNull(WorkspaceFactories.Open("@C:\\*.sde"));
        }

        [TestMethod]
        public void WorkspaceFactories_Open_NotNull()
        {
            Assert.IsNotNull(WorkspaceFactories.Open(Settings.Default.TestData));
        }

        [TestMethod]
        [ExpectedException(typeof (NotSupportedException))]
        public void WorkspaceFactories_Open_NotSupportedException()
        {
            Assert.IsNull(WorkspaceFactories.Open(""));
        }

        #endregion
    }
}