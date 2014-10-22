using System.Collections.Generic;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wave.Extensions.Esri.Tests.Properties;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public abstract class EsriTests
    {
        #region Fields

        private ComReleaser _ComReleaser;
        private IMap _Map;
        private EsriRuntimeAuthorization _RuntimeAuthorization;
        private IWorkspace _Workspace;

        #endregion

        #region Protected Properties

        protected ComReleaser ComReleaser
        {
            get { return _ComReleaser; }
        }

        protected IWorkspace Workspace
        {
            get { return _Workspace; }
        }

        #endregion

        #region Public Methods

        [TestCleanup]
        public virtual void Cleanup()
        {
            _RuntimeAuthorization.Dispose();
            _ComReleaser.Dispose();
            _Workspace = null;
        }

        [TestInitialize]
        public virtual void Setup()
        {
            _ComReleaser = new ComReleaser();
            _RuntimeAuthorization = new EsriRuntimeAuthorization();

            Assert.IsTrue(_RuntimeAuthorization.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcEditor));

            _Workspace = WorkspaceFactories.Open(Settings.Default.TestData);
            _ComReleaser.ManageLifetime(_Workspace);
        }

        #endregion

        #region Protected Methods

        protected virtual IMap CreateMap()
        {
            if (_Map != null)
                return _Map;

            _Map = new MapClass();
            _ComReleaser.ManageLifetime(_Map);

            foreach (var o in this.GetTestClasses())
            {
                IFeatureLayer layer = new FeatureLayerClass();
                layer.FeatureClass = o;
                layer.Name = o.AliasName;

                _Map.AddLayer(layer);
            }

            return _Map;
        }

        protected virtual IFeatureClass GetTestClass()
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            IFeatureClass testClass = fws.OpenFeatureClass("TRANSFORMER");

            _ComReleaser.ManageLifetime(testClass);

            return testClass;
        }

        protected virtual IEnumerable<IFeatureClass> GetTestClasses()
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            var names = new[] {"TRANSFORMER", "ANCHORGUY"};
            foreach (var name in names)
            {
                IFeatureClass testClass = fws.OpenFeatureClass(name);
                Assert.IsNotNull(testClass);

                _ComReleaser.ManageLifetime(testClass);

                yield return testClass;
            }
        }

        protected virtual ITable GetTestTable()
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            ITable testTable = fws.OpenTable("ASSEMBLY");

            _ComReleaser.ManageLifetime(testTable);

            return testTable;
        }

        #endregion
    }
}