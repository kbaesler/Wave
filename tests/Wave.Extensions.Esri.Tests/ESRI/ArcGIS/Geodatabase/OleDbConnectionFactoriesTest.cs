using System.Data;
using System.IO;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wave.Extensions.Esri.Tests.Properties;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class OleDbConnectionFactoriesTest
#if V10
        : EsriTests
#endif
    {
        [TestMethod]
        [TestCategory("ESRI")]
        public void OleDbConnectionFactories_GetFileGdbConnection_IsNotNull()
        {
            Assert.IsNotNull(OleDbConnectionFactories.GetFileGdbConnection(Path.GetFullPath(Settings.Default.Minerville)));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void OleDbConnectionFactories_GetFileGdbConnection_Open()
        {
            using (var connection = OleDbConnectionFactories.GetFileGdbConnection(Path.GetFullPath(Settings.Default.Minerville)))
            {
                connection.Open();
                Assert.IsTrue(connection.State == ConnectionState.Open);
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void WorkspaceFactories_GetDbConnection_ExecuteNonQuery()
        {
            using (var connection = OleDbConnectionFactories.GetFileGdbConnection(Path.GetFullPath(Settings.Default.Minerville)))
            {
                connection.Open();
                Assert.IsTrue(connection.State == ConnectionState.Open);
            }
        }
    }
}