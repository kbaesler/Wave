using System;
using System.Data;
using System.IO;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wave.Extensions.Esri.Tests.Properties;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class OleDbConnectionFactoriesTest
    {
        [TestMethod]
        [TestCategory("ESRI")]
        public void OleDbConnectionFactories_GetFileGdbConnection_IsNull()
        {
            Assert.IsNull(OleDbConnectionFactories.GetFileGdbConnection(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.Minerville))));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void OleDbConnectionFactories_GetFileGdbConnection_Open()
        {
            using (var connection = OleDbConnectionFactories.GetFileGdbConnection(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.Minerville))))
            {
                connection.Open();
                Assert.IsTrue(connection.State == ConnectionState.Open);
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void WorkspaceFactories_GetDbConnection_ExecuteNonQuery()
        {
            using (var connection = OleDbConnectionFactories.GetFileGdbConnection(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.Minerville))))
            {
                connection.Open();
                Assert.IsTrue(connection.State == ConnectionState.Open);
                var value = connection.ExecuteReader("SELECT COUNT(*) FROM ASSEMBLY");
            }
        }
    }
}
