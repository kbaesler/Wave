using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;
using Miner.Interop;
using Miner.Interop.Process;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class WorkRequestTest : ProcessTests
    {
        #region Constructors

        public WorkRequestTest()
            : base(mmProductInstallation.mmPIDesigner)
        {
        }

        #endregion

        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxWorkRequest_CreateNew_IsTrue()
        {
            using (WorkRequest request = new WorkRequest(base.PxApplication))
            {
                Assert.AreEqual(true, request.CreateNew(base.PxApplication.User));
                Assert.AreEqual(base.PxApplication.User.Name, request.Owner.Name);

                request.Delete();
            }
        }


        [TestMethod]
        [TestCategory("Miner")]
        public void IPxWorkRequest_CustomerUpdate_Valid()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (WorkRequest request = new WorkRequest(base.PxApplication))
                {
                    Assert.AreEqual(true, request.Initialize(nodeID));

                    if (request.Customer != null && request.Customer.Valid)
                    {
                        request.Customer.FirstName = "John";
                        request.Customer.LastName = "Doe";
                        request.Customer.Update();

                        Assert.AreEqual("John", request.Customer.FirstName);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxWorkRequest_Initialize_IsTrue()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (WorkRequest request = new WorkRequest(base.PxApplication))
                {
                    Assert.AreEqual(true, request.Initialize(nodeID));
                }
            }
        }


        [TestMethod]
        [TestCategory("Miner")]
        public void IPxWorkRequest_Location_Updates_Valid()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (WorkRequest request = new WorkRequest(base.PxApplication))
                {
                    Assert.AreEqual(true, request.Initialize(nodeID));

                    if (request.Location != null && request.Location.Valid)
                    {
                        request.Location.FacilityDisplayField = "FacilityID";
                        Assert.AreEqual("FacilityID", request.Location.FacilityDisplayField);

                        if (request.Location.Address != null && request.Location.Address.Valid)
                        {
                            request.Location.Address.Address1 = " 380 New York Street, Redlands, CA 92373-8100";
                            Assert.AreEqual(" 380 New York Street, Redlands, CA 92373-8100", request.Location.Address.Address1);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxWorkRequest_Version_IsNotNull()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (WorkRequest session = new WorkRequest(base.PxApplication))
                {
                    Assert.IsTrue(session.Initialize(nodeID));
                    Assert.IsNotNull(session.Version);
                }
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxWorkRequest_GetVersionStatus_IsNotNull()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (WorkRequest session = new WorkRequest(base.PxApplication))
                {
                    Assert.IsTrue(session.Initialize(nodeID));
                    Assert.IsNotNull(session.Version);
                    Assert.IsInstanceOfType(session.Version.GetVersionStatus(), typeof(PxVersionStatus));
                }
            }
        }
        #endregion
    }
}