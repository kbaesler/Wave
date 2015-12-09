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
        [TestCategory("Miner")]
        public void IMap_GetFeatureClass_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureClass("DISTRIBUTIONTRANSFORMER");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetFeatureClass_IsNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureClass("UNITTEST", false);
            Assert.IsNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IMap_GetFeatureClass_MissingClassModelNameException()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureClass("UNITTEST");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetFeatureClasses_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureClasses("DISTRIBUTIONTRANSFORMER");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetFeatureLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureLayer("DISTRIBUTIONTRANSFORMER");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetFeatureLayer_IsNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureLayer("UNITTEST", false);
            Assert.IsNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IMap_GetFeatureLayer_MissingClassModelNameException()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureLayer("UNITTEST");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetFeatureLayers_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetFeatureLayers("DISTRIBUTIONTRANSFORMER");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetTable_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetTable("ASSEMBLY");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingClassModelNameException))]
        public void IMap_GetTable_MissingClassModelNameException()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetTable("1238123mva0dg18231");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetTables_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.GetTables("ASSEMBLY");
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetWorkpace_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var workspace = map.GetWorkspace("MM ENTERPRISE");
            Assert.IsNotNull(workspace);
        }

        [TestMethod]
        [TestCategory("Miner")]
        [ExpectedException(typeof (MissingDatabaseModelNameException))]
        public void IMap_GetWorkpace_MissingDatabaseModelNameException()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var workspace = map.GetWorkspace("0912312alla1nl01");
            Assert.IsNotNull(workspace);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMap_GetWorkpace_Predicate_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var workspace = map.GetWorkspace();
            Assert.IsNotNull(workspace);
        }

        #endregion
    }
}