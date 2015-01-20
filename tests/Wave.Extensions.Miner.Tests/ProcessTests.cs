using System;
using System.IO;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;
using Miner.Interop;
using Miner.Interop.Process;

using Wave.Extensions.Miner.Tests.Properties;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class ProcessTests : MinerTests
    {
        #region Fields

        private readonly mmProductInstallation _ProductInstallation;

        private IMMPxApplication _PxApplication;

        #endregion

        #region Constructors

        public ProcessTests(mmProductInstallation productInstallation)
        {
            _ProductInstallation = productInstallation;
        }

        #endregion

        #region Protected Properties

        protected IMMPxApplication PxApplication
        {
            get { return _PxApplication; }
        }

        #endregion

        #region Public Methods

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();

            if (_PxApplication != null)
                _PxApplication.Shutdown();
        }

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            var factory = PxApplicationFactories.GetFactory(DBMS.Access);

            if (_ProductInstallation != mmProductInstallation.mmPIDesigner)
            {
                _PxApplication = factory.Open("adams", "", Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,Settings.Default.SessionManager)), "", false, ArcFM.Process.SessionManager.Name);
            }
            else
            {
                _PxApplication = factory.Open("adams", "", Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.WorkflowManager)), "", false, ArcFM.Process.WorkflowManager.Name);
            }

            ((IMMPxApplicationEx2) _PxApplication).Workspace = base.Workspace;
        }

        #endregion
    }
}