using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;
using Miner.Interop;
using Miner.Interop.Process;

using Wave.Extensions.Miner.Tests;

namespace SE.Tests.Process
{
    public class TestDesign : Design
    {
        #region Constructors

        public TestDesign(IMMPxApplication pxApplication)
            : base(pxApplication)
        {
        }

        #endregion

        #region Public Properties

        public bool IsNew { get; set; }

        #endregion

        #region Public Methods

        public override bool Initialize(int nodeID)
        {
            this.IsNew = true;

            return base.Initialize(nodeID);
        }

        #endregion
    }

    [TestClass]
    public class DesignTest : ProcessTests
    {
        #region Constructors

        public DesignTest()
            : base(mmProductInstallation.mmPIDesigner)
        {
        }

        #endregion

        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxApplication_GetDesign_IsNull()
        {
            TestDesign design = base.PxApplication.GetDesign((nodeID, sender) =>
            {
                Assert.Fail("The predicate function shouldn't be called.");

                var o = new TestDesign(sender);
                o.Initialize(nodeID);
                return o;
            });

            Assert.IsNull(design);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxDesign_CreateNew_IsTrue()
        {
            using (Design design = new Design(base.PxApplication))
            {
                Assert.IsTrue(design.CreateNew(base.PxApplication.User));
                Assert.AreEqual(base.PxApplication.User.Name, design.Owner.Name);

                design.Delete();
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxDesign_Dispose_IsNotNull()
        {
            IMMPxNode node;
            using (Design design = new Design(base.PxApplication))
            {
                design.CreateNew(base.PxApplication.User);
                node = design.Node;
                design.Delete();
            }

            Assert.IsNotNull(node);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IPxDesign_GetDesignXml_IsNotNull()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.Design));
            if (table.Rows.Count > 1)
            {
                int nodeID = table.Rows[1].Field<int>(0);
                using (Design design = new Design(base.PxApplication))
                {
                    Assert.IsTrue(design.Initialize(nodeID));

                    string xml = design.GetDesignXml();
                    Assert.IsNotNull(xml);
                }
            }
        }


        [TestMethod]
        [TestCategory("Miner")]
        public void IPxDesign_Initialize_IsNew_IsTrue()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.Design));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (TestDesign design = new TestDesign(base.PxApplication))
                {
                    Assert.IsTrue(design.Initialize(nodeID));
                    Assert.IsTrue(design.IsNew);
                }
            }
        }


        [TestMethod]
        [TestCategory("Miner")]
        public void IPxDesign_Initialize_IsTrue()
        {
            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.Design));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (Design design = new Design(base.PxApplication))
                {
                    Assert.IsTrue(design.Initialize(nodeID));
                }
            }
        }

        #endregion
    }
}