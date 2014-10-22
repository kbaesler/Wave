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
        public void IPxWorkRequest_CreateNew_IsTrue()
        {
            IPxWorkRequest request = new WorkRequest(base.PxApplication);
            Assert.AreEqual(true, request.CreateNew(base.PxApplication.User));
            Assert.AreEqual(base.PxApplication.User.Name, request.Owner.Name);

            request.Delete();
        }


        [TestMethod]
        public void IPxWorkRequest_CustomerUpdate_Valid()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                IPxWorkRequest request = new WorkRequest(base.PxApplication);
                Assert.AreEqual(true, request.Initialize(nodeID));

                request.Customer.FirstName = "John";
                request.Customer.LastName = "Doe";
                request.Customer.Update();

                Assert.AreEqual("John", request.Customer.FirstName);
            }
        }

        [TestMethod]
        public void IPxWorkRequest_Initialize_IsTrue()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                IPxWorkRequest request = new WorkRequest(base.PxApplication);
                Assert.AreEqual(true, request.Initialize(nodeID));
            }
        }


        [TestMethod]
        public void IPxWorkRequest_Location_Updates_Valid()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.WorkRequest));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                IPxWorkRequest request = new WorkRequest(base.PxApplication);
                Assert.AreEqual(true, request.Initialize(nodeID));

                request.Location.FacilityDisplayField = "FacilityID";
                Assert.AreEqual("FacilityID", request.Location.FacilityDisplayField);

                request.Location.Address.Address1 = " 380 New York Street, Redlands, CA 92373-8100";
                Assert.AreEqual(" 380 New York Street, Redlands, CA 92373-8100", request.Location.Address.Address1);
            }
        }

        #endregion
    }
}