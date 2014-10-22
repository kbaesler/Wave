using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class MapExtensionsTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        public void IMap_GetFeatureClass_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureClass("DISTRIBUTIONTRANSFORMER");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void IMap_GetFeatureClass_IsNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureClass("UNITTEST", false);
            Assert.IsNull(layer);
        }

        [TestMethod]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IMap_GetFeatureClass_MissingClassModelNameException()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureClass("UNITTEST");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void IMap_GetFeatureLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureLayer("DISTRIBUTIONTRANSFORMER");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void IMap_GetFeatureLayer_IsNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureLayer("UNITTEST", false);
            Assert.IsNull(layer);
        }

        [TestMethod]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IMap_GetFeatureLayer_MissingClassModelNameException()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureLayer("UNITTEST");
            Assert.IsNotNull(layer);
        }

        #endregion
    }
}