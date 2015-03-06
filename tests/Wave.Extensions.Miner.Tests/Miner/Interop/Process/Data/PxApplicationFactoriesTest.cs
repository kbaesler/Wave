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
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Access);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (NotSupportedException))]
        public void PxApplicationFactories_GetFactory_Db2_NotSupportedException()
        {
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Db2);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (NotSupportedException))]
        public void PxApplicationFactories_GetFactory_File_NotSupportedException()
        {
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.File);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (NotSupportedException))]
        public void PxApplicationFactories_GetFactory_Informix_NotSupportedException()
        {
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Informix);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void PxApplicationFactories_GetFactory_Oracle_IsNotNull()
        {
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Oracle);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (NotSupportedException))]
        public void PxApplicationFactories_GetFactory_PostgreSql_NotSupportedException()
        {
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.PostgreSql);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void PxApplicationFactories_GetFactory_SqlServer_IsNotNull()
        {
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.SqlServer);
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (NotSupportedException))]
        public void PxApplicationFactories_GetFactory_Uknown_NotSupportedException()
        {
            PxApplicationFactory factory = PxApplicationFactories.GetFactory(DBMS.Unknown);
            Assert.IsNotNull(factory);
        }

        #endregion
    }
}