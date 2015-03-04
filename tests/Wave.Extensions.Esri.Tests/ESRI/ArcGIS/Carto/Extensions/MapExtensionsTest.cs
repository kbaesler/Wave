using System;

using ESRI.ArcGIS.Carto;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class MapExtensionsTest : EsriTests
    {
        #region Public Methods
        [TestMethod]
        public void IMap_Where_ICoverageAnnotationLayer2_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<ICoverageAnnotationLayer2>(l => l != null);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        public void IMap_Where_ICoverageAnnotationLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<ICoverageAnnotationLayer>(l => l != null);
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void IMap_Where_IDataLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IDataLayer>(l => l != null);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        public void IMap_Where_IDataLayer2_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IDataLayer2>(l => l != null);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        public void IMap_Where_IFDOGraphicsLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IFDOGraphicsLayer>(l => l != null);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        public void IMap_Where_IFDOGraphicsLayer2_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IFDOGraphicsLayer2>(l => l != null);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        public void IMap_Where_IFeatureLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IFeatureLayer>(l => l.Valid);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        public void IMap_Where_IFeatureLayer2_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IFeatureLayer2>(l => l != null);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        public void IMap_Where_IGraphicsLayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IGraphicsLayer>(l => l != null);
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void IMap_Where_ILayer_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<ILayer>(l => l.Valid);
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void IMap_Where_ILayer2_IsNotNull()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<ILayer2>(l => l.Valid);
            Assert.IsNotNull(layer);
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void IMap_Where_NotSupportedException()
        {
            IMap map = this.CreateMap();
            Assert.IsNotNull(map);

            var layer = map.Where<IQuerySize>(l => l != null);
            Assert.IsNotNull(layer);
        }
        #endregion
    }
}