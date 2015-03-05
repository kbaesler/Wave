using System;
using System.Collections.Generic;
using System.IO;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wave.Extensions.Esri.Tests.Properties;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public abstract class EsriTests : IDisposable
    {
        #region Fields

        private ComReleaser _ComReleaser;
        private IMap _Map;
        private EsriRuntimeAuthorization _RuntimeAuthorization;
        private IWorkspace _Workspace;

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the COM releaser.
        /// </summary>
        /// <value>
        ///     The COM releaser.
        /// </value>
        protected ComReleaser ComReleaser
        {
            get { return _ComReleaser; }
        }

        /// <summary>
        ///     Gets the workspace.
        /// </summary>
        /// <value>
        ///     The workspace.
        /// </value>
        protected IWorkspace Workspace
        {
            get { return _Workspace; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public virtual void Cleanup()
        {
            if (_RuntimeAuthorization != null)
            {
                _RuntimeAuthorization.Dispose();
                _RuntimeAuthorization = null;
            }

            if (_ComReleaser != null)
            {
                _ComReleaser.Dispose();
                _ComReleaser = null;
            }

            _Workspace = null;
        }

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        [TestInitialize]
        public virtual void Setup()
        {
            _ComReleaser = new ComReleaser();
            _RuntimeAuthorization = new EsriRuntimeAuthorization();

            Assert.IsTrue(_RuntimeAuthorization.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcEditor));

            _Workspace = WorkspaceFactories.Open(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.Minerville)));
            _ComReleaser.ManageLifetime(_Workspace);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the map.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     Gets the test class.
        /// </summary>
        /// <returns></returns>
        protected virtual IFeatureClass GetTestClass()
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            IFeatureClass testClass = fws.OpenFeatureClass("TRANSFORMER");

            _ComReleaser.ManageLifetime(testClass);

            return testClass;
        }

        /// <summary>
        ///     Gets the test classes.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     Gets the test table.
        /// </summary>
        /// <returns></returns>
        protected virtual ITable GetTestTable()
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            ITable testTable = fws.OpenTable("ASSEMBLY");

            _ComReleaser.ManageLifetime(testTable);

            return testTable;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ComReleaser != null)
                    _ComReleaser.Dispose();

                _ComReleaser = null;

                if (_RuntimeAuthorization != null)
                    _RuntimeAuthorization.Dispose();

                _RuntimeAuthorization = null;
            }
        }

        #endregion
    }
}