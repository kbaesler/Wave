using System.Linq;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class LayerExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void IEnumLayer_AsEnumerable_Count_Equals_2()
        {
            IEnumLayer layers = this.CreateLayers();
            Assert.IsNotNull(layers);

            var items = layers.AsEnumerable();
            Assert.AreEqual(2, items.Count());
        }

        [TestMethod]
        public void IFeatureLayer_Identify_Any()
        {
            IEnumLayer layers = this.CreateLayers();
            Assert.IsNotNull(layers);

            var layer = layers.AsEnumerable().OfType<IFeatureLayer>().First(o => o.Valid);
            IFeature feature = layer.FeatureClass.Fetch(1, 2, 3, 4, 5, 6).FirstOrDefault();
            Assert.IsNotNull(feature);

            var features = layer.Identify(feature.Shape);
            Assert.IsTrue(features.Any());
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the layers.
        /// </summary>
        /// <returns></returns>
        private IEnumLayer CreateLayers()
        {
            IMap map = new MapClass();

            foreach (var testClass in base.GetTestClasses())
            {
                IFeatureLayer layer = new FeatureLayerClass();
                layer.FeatureClass = testClass;
                layer.Name = testClass.AliasName;

                map.AddLayer(layer);
            }

            return map.Layers[null, true];
        }

        #endregion
    }
}