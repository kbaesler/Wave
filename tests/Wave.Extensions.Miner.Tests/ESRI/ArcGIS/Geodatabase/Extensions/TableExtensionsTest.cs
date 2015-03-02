using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class TableExtensionsTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        public void ITable_GetPrimaryDisplayField_NotNull()
        {
            var table = base.GetTestTable();
            Assert.IsNotNull(table);

            var field = table.GetPrimaryDisplayField();
            Assert.IsNotNull(field);
        }

        #endregion
    }
}