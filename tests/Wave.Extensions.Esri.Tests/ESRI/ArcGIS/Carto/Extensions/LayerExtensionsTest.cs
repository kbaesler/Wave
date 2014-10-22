using System.Linq;

using ESRI.ArcGIS.Carto;

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
        public void IEnumLayer_Where_IsNotNull()
        {
            IEnumLayer layers = this.CreateLayers();
            Assert.IsNotNull(layers);

            var valid = layers.Where(o => o.Valid);
            Assert.IsNotNull(valid);
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