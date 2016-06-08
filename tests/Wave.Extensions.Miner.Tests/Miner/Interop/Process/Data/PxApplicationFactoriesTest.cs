using System;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop.Process;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class PxApplicationFactoriesTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void PxApplicationFactories_GetFactory_Access_IsNotNull()
        {
            IPxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Access);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (NotSupportedException))]
        public void PxApplicationFactories_GetFactory_NotSupportedException()
        {
            IPxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Unknown);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void PxApplicationFactories_GetFactory_Oracle_IsNotNull()
        {
            IPxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Oracle);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void PxApplicationFactories_GetFactory_SqlServer_IsNotNull()
        {
            IPxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.SqlServer);
            Assert.IsNotNull(factory);
        }

        #endregion
    }
}