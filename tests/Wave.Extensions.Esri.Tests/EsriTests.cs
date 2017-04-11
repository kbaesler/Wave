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

        private readonly string _PathName;

        private IMap _Map;
        private EsriRuntimeAuthorization _RuntimeAuthorization;

        #endregion

        #region Constructors

        protected EsriTests()
            : this(Path.GetFullPath(Settings.Default.Minerville))
        {
        }

        protected EsriTests(string pathName)
        {
            _PathName = pathName;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the COM releaser.
        /// </summary>
        /// <value>
        ///     The COM releaser.
        /// </value>
        protected ComReleaser ComReleaser { get; private set; }

        /// <summary>
        ///     Gets the workspace.
        /// </summary>
        /// <value>
        ///     The workspace.
        /// </value>
        protected IWorkspace Workspace { get; private set; }

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

            if (ComReleaser != null)
            {
                ComReleaser.Dispose();
                ComReleaser = null;
            }

            Workspace = null;
        }

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        [TestInitialize]
        public virtual void Setup()
        {
            ComReleaser = new ComReleaser();
            _RuntimeAuthorization = new EsriRuntimeAuthorization();

            Assert.IsTrue(_RuntimeAuthorization.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard));

            Workspace = WorkspaceFactories.Open(_PathName);
            ComReleaser.ManageLifetime(Workspace);
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
            ComReleaser.ManageLifetime(_Map);

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
        /// Gets the feature class with the name,
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected virtual IFeatureClass GetFeatureClass(string tableName)
        {
            IFeatureWorkspace fws = (IFeatureWorkspace)this.Workspace;
            IFeatureClass testClass = fws.OpenFeatureClass(tableName);

            ComReleaser.ManageLifetime(testClass);

            return testClass;
        }

        /// <summary>
        ///     Gets the test class.
        /// </summary>
        /// <returns></returns>
        protected virtual IFeatureClass GetTestClass()
        {
            return this.GetFeatureClass("TRANSFORMER");
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

                ComReleaser.ManageLifetime(testClass);

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

            ComReleaser.ManageLifetime(testTable);

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
                if (ComReleaser != null)
                    ComReleaser.Dispose();

                ComReleaser = null;

                if (_RuntimeAuthorization != null)
                    _RuntimeAuthorization.Dispose();

                _RuntimeAuthorization = null;
            }
        }

        #endregion
    }
}