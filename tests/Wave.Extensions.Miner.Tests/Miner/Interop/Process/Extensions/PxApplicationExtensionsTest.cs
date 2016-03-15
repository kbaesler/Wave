using System;
using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;
using Miner.Interop;
using Miner.Interop.Process;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class PxApplicationExtensionsTest : ProcessTests
    {
        #region Constructors

        public PxApplicationExtensionsTest()
            : base(mmProductInstallation.mmPIArcFM)
        {
        }

        #endregion

        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_AddHistory_IsNotNull()
        {
            var table = base.PxApplication.ExecuteQuery("SELECT session_id FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.SessionManager.Tables.Session));
            if (table.Rows.Count > 0)
            {
                using (var session = new Session(base.PxApplication, table.Rows[0].Field<int>(0)))
                {
                    var history = base.PxApplication.AddHistory(session.Node, string.Format("Unit tested on {0}", DateTime.Now.ToShortDateString()), "");
                    Assert.IsNotNull(history);
                }             
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_ExecuteScalar()
        {
            int count = base.PxApplication.ExecuteScalar(string.Format("SELECT COUNT(*) FROM {0}", base.PxApplication.GetQualifiedTableName(ArcFM.Process.SessionManager.Tables.Session)), -1);
            Assert.AreNotEqual(-1, count);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetCurrentNode_IsNull()
        {
            IMMPxNode node = base.PxApplication.GetCurrentNode();
            Assert.IsNull(node);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetFilterByName_IsNotNull()
        {
            IMMPxFilter filter = base.PxApplication.GetFilter(ArcFM.Process.SessionManager.Filters.AllSessions);
            Assert.IsNotNull(filter);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetHistory_IsNotNull()
        {
            var table = base.PxApplication.ExecuteQuery("SELECT session_id FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.SessionManager.Tables.Session));
            if (table.Rows.Count > 0)
            {
                using (var session = new Session(base.PxApplication, table.Rows[0].Field<int>(0)))
                {
                    var history = base.PxApplication.GetHistory(session.Node);
                    Assert.IsNotNull(history);
                }
            }
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetHistory_IsNull()
        {
            var history = base.PxApplication.GetHistory(0, 0);
            Assert.AreEqual("<NODE_HISTORY/>\r\n", history.Xml);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetQualifiedTableName()
        {
            string tableName = base.PxApplication.GetQualifiedTableName(ArcFM.Process.SessionManager.Tables.Session);
            Assert.AreEqual(ArcFM.Process.SessionManager.Tables.Session, tableName);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetStateByID_IsNotNull()
        {
            IMMPxState state = base.PxApplication.GetState(1);
            Assert.IsNotNull(state);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetStateByName_IsNotNull()
        {
            IMMPxState state = base.PxApplication.GetState("In Progress");
            Assert.IsNotNull(state);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetUserByID_IsNotNull()
        {
            IMMPxUser user = base.PxApplication.GetUser(base.PxApplication.User.Id);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_GetUserByName_IsNotNull()
        {
            IMMPxUser user = base.PxApplication.GetUser("adams");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxApplication_IsValidTransition_IsFalse()
        {
            IMMPxState state = base.PxApplication.GetState("In Progress");
            bool valid = base.PxApplication.Transitions.IsValidTransition(base.PxApplication.User, state, state);
            Assert.IsFalse(valid);
        }

        #endregion
    }
}