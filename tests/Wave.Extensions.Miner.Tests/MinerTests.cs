using System.Collections.Generic;
using System.Diagnostics;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop;

using Wave.Extensions.Esri.Tests;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class MinerTests : EsriTests
    {
        #region Fields

        private IMap _Map;
        private RuntimeAuthorization _RuntimeAuthorization;

        #endregion

        #region Public Methods       

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();

            _RuntimeAuthorization.Dispose();
        }

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _RuntimeAuthorization = new RuntimeAuthorization();

            Assert.IsTrue(_RuntimeAuthorization.Initialize(mmLicensedProductCode.mmLPArcFM));
        }

        #endregion

        #region Protected Methods

        protected override IMap CreateMap()
        {
            if (_Map != null)
                return _Map;

            _Map = new MapClass();

            foreach (var o in base.GetTestClasses())
            {
                IFeatureLayer layer = new FeatureLayerClass();
                layer.FeatureClass = o;
                layer.Name = o.AliasName;

                _Map.AddLayer(layer);
            }

            var table = base.GetTestTable();
            IStandaloneTable standaloneTable = new StandaloneTableClass();
            standaloneTable.Table = table;
            standaloneTable.Name = ((IDataset) table).Name;

            IStandaloneTableCollection collection = (IStandaloneTableCollection) _Map;
            collection.AddStandaloneTable(standaloneTable);

            return _Map;
        }

        protected override IEnumerable<IFeatureClass> GetTestClasses()
        {
            foreach (var testClass in  base.Workspace.GetFeatureClasses("DISTRIBUTIONTRANSFORMER", "ANCHORGUY"))
            {
                base.ComReleaser.ManageLifetime(testClass);

                yield return testClass;
            }
        }

        #endregion
    }
}