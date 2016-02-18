using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;
using Miner.Interop;
using Miner.Interop.Process;

namespace Wave.Extensions.Miner.Tests.Process
{
    public class TestDesign : Design
    {
        #region Constructors

        public TestDesign(IMMPxApplication pxApplication, int nodeId)
            : base(pxApplication, nodeId)
        {
        }

        #endregion

        #region Public Properties

        public bool IsNew { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Creates the process framework node wrapper for the specified the <paramref name="user" />.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="user">The current user.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully created; otherwise <c>false</c>.
        /// </returns>
        protected override bool Initialize(IMMWorkflowManager extension, IMMPxUser user)
        {
            this.IsNew = true;

            return base.Initialize(extension, user);
        }

        /// <summary>
        ///     Initializes the process framework node wrapper using the specified <paramref name="nodeId" /> for the node.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> if the node was successfully initialized; otherwise
        ///     <c>false</c>.
        /// </returns>
        protected override bool Initialize(IMMWorkflowManager extension, int nodeId)
        {
            this.IsNew = false;

            return base.Initialize(extension, nodeId);
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

                var o = new TestDesign(sender, nodeID);
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
                Assert.IsTrue(design.Valid);
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
                int nodeID = table.Rows[0].Field<int>(0);
                using (Design design = new Design(base.PxApplication, nodeID))
                {
                    Assert.IsTrue(design.Valid);

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
                using (TestDesign design = new TestDesign(base.PxApplication, nodeID))
                {
                    Assert.IsFalse(design.IsNew);
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
                using (Design design = new Design(base.PxApplication, nodeID))
                {
                    Assert.IsTrue(design.Valid);
                }
            }
        }

        #endregion
    }
}