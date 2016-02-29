using System.Linq;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class LayerAsyncExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeatureLayer_IdentifyAsync_Any()
        {
            IEnumLayer layers = this.CreateLayers();
            Assert.IsNotNull(layers);

            var layer = layers.AsEnumerable().OfType<IFeatureLayer>().First(o => o.Valid);
            var task = layer.FeatureClass.FetchAsync(1, 2, 3, 4, 5, 6);
            var features = task.GetAwaiter().GetResult();
            Assert.IsNotNull(features);

            var feature = features.First();
            var identifies = layer.Identify(feature.Shape);
            Assert.IsTrue(identifies.Any());
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