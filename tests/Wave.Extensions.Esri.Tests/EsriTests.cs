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

        /// <summary>
        ///     Initializes a new instance of the <see cref="EsriTests" /> class.
        /// </summary>
        protected EsriTests()
            : this(Path.GetFullPath(Settings.Default.Roadways))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EsriTests" /> class.
        /// </summary>
        /// <param name="pathName">Name of the path.</param>
        protected EsriTests(string pathName)
        {
            _PathName = Path.GetFullPath(pathName);
        }

        #endregion

        #region Protected Properties

        protected virtual string[] FeatureClassNames
        {
            get { return new[] {this.LineFeatureClass, this.PointFeatureClass}; }
        }

        protected abstract string LineFeatureClass { get; }
        protected abstract string PointFeatureClass { get; }
        protected abstract string RelationshipClass { get; }
        protected abstract string TableName { get; }
        protected ComReleaser ComReleaser { get; private set; }
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

        protected virtual IFeatureClass GetFeatureClass(string tableName)
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            IFeatureClass testClass = fws.OpenFeatureClass(tableName);

            ComReleaser.ManageLifetime(testClass);

            return testClass;
        }

        protected IFeatureClass GetLineFeatureClass()
        {
            return this.GetFeatureClass(this.LineFeatureClass);
        }

        protected IFeatureClass GetPointFeatureClass()
        {
            return this.GetFeatureClass(this.PointFeatureClass);
        }

        protected virtual ITable GetTable()
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            ITable testTable = fws.OpenTable(this.TableName);

            ComReleaser.ManageLifetime(testTable);

            return testTable;
        }

        protected virtual IEnumerable<IFeatureClass> GetTestClasses()
        {
            IFeatureWorkspace fws = (IFeatureWorkspace) this.Workspace;
            foreach (var name in this.FeatureClassNames)
            {
                IFeatureClass testClass = fws.OpenFeatureClass(name);
                Assert.IsNotNull(testClass);

                ComReleaser.ManageLifetime(testClass);

                yield return testClass;
            }
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

    [TestClass]
    public abstract class MinervilleTests : EsriTests
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MinervilleTests" /> class.
        /// </summary>
        protected MinervilleTests()
            : base(Settings.Default.Minerville)
        {
        }

        #endregion

        #region Protected Properties

        protected override string LineFeatureClass
        {
            get { return "TRANSFORMER"; }
        }

        protected override string PointFeatureClass
        {
            get { return "ANCHORGUY"; }
        }

        protected override string TableName
        {
            get { return "ASSEMBLY"; }
        }

        #endregion
    }

    [TestClass]
    public abstract class RoadwaysTests : EsriTests
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoadwaysTests" /> class.
        /// </summary>
        protected RoadwaysTests()
            : base(Settings.Default.Roadways)
        {
        }

        #endregion

        #region Protected Properties

        protected override string LineFeatureClass
        {
            get { return "CENTERLINE"; }
        }

        protected override string PointFeatureClass
        {
            get { return "CALIBRATION_POINT"; }
        }

        protected override string RelationshipClass
        {
            get { return "Redline__ATTACHREL"; }
        }

        protected override string TableName
        {
            get { return "CRASHES"; }
        }

        #endregion
    }
}