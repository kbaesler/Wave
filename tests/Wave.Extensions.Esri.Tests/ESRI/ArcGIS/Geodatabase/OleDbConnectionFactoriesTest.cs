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
        : RoadwaysTests
#endif
    {
        [TestMethod]
        [TestCategory("ESRI")]
        public void OleDbConnectionFactories_GetFileGdbConnection_IsNotNull()
        {
            Assert.IsNotNull(OleDbConnectionFactories.GetFileGdbConnection(Path.GetFullPath(Settings.Default.Minerville), OleDbGeometryType.Wkb));
        }
    }
}