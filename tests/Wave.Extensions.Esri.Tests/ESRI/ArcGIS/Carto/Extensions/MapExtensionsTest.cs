using System.Linq;

using ESRI.ArcGIS.Carto;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class MapExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void IMap_Where_Valid_Count_Equals_2()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where(o => o.Valid);
            Assert.IsNotNull(layer);
            Assert.AreEqual(2, layer.Count());
        }

        #endregion
    }
}