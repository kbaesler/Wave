using ESRI.ArcGIS.Carto;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class MapExtensionsTest : RoadwaysTests
    {
        #region Public Methods
        public void IMap_GetFeatureLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var testClass = base.GetLineFeatureClass();
            var layer = map.GetFeatureLayer(testClass);
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IMap_GetFeatureLayers_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var testClass = base.GetLineFeatureClass();
            var layer = map.GetFeatureLayers(testClass);
            Assert.IsNotNull(layer);
        }
    
        [TestMethod]
        [TestCategory("ESRI")]
        public void IMap_Where_IFeatureLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetLayers<IFeatureLayer>(l => l.Valid);
            Assert.IsNotNull(layer);
        }
            
        [TestMethod]
        [TestCategory("ESRI")]
        public void IMap_Where_ILayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetLayers<ILayer>(l => l.Valid);
            Assert.IsNotNull(layer);
        }

        #endregion
    }
}