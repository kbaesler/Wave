using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop;

namespace Wave.Extensions.Miner.Tests
{
    /// <summary>
    ///     Provides unit tests for the <see cref="D8ListExtensions" /> class implementations.
    /// </summary>
    [TestClass]
    public class D8ListClassExtensionsTest
    {
        #region Fields

        private string[] _LayerNames;
        private ID8List _List;

        #endregion

        #region Public Methods

        [TestMethod]
        public void ID8List_Count_Equals_12()
        {
            int count = _List.Count();
            Assert.AreEqual(12, count);
        }

        [TestMethod]
        public void ID8List_Where_Equals_9()
        {
            var iter = _List.Where(o => o.ItemType == mmd8ItemType.mmd8itFeature);
            Assert.AreEqual(9, iter.Count());
        }

        /// <summary>
        ///     Creates the <see cref="ID8List" /> that is used for the unit tests.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _List = new D8ListClass();
            _LayerNames = new[] {"Support Structure", "Surface Structure", "Underground Structure"};

            foreach (var layerName in _LayerNames)
            {
                ID8Layer layer = new D8LayerClass();
                layer.LayerName = layerName;

                string tableName = layerName.Replace(" ", "");
                for (int i = 0; i < _LayerNames.Length; i++)
                {
                    ID8Feature feature = new D8FeatureClass();
                    feature.Name = tableName;

                    ID8GeoAssoc geoAssoc = (ID8GeoAssoc) feature;
                    geoAssoc.OID = i;
                    geoAssoc.TableName = tableName;

                    ((ID8List) layer).Add((ID8ListItem) feature);
                }

                _List.Add((ID8ListItem) layer);
            }
        }

        #endregion
    }
}