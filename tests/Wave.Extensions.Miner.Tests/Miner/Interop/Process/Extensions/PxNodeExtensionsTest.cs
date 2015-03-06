using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner;
using Miner.Interop;
using Miner.Interop.Process;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class PxNodeExtensionsTest : ProcessTests
    {
        #region Fields

        private IMMPxNode _Node;

        #endregion

        #region Constructors

        public PxNodeExtensionsTest()
            : base(mmProductInstallation.mmPIDesigner)
        {
        }

        #endregion

        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxNode_GetTaskByID()
        {
            var task1 = _Node.GetTask(ArcFM.Process.WorkflowManager.Tasks.OpenDesign);
            var task2 = _Node.GetTask(task1.TaskID);
            Assert.AreEqual(task1, task2);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxNode_GetTaskByName()
        {
            var task = _Node.GetTask(ArcFM.Process.WorkflowManager.Tasks.OpenDesign);
            Assert.IsNotNull(task);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxNode_GetTopLevelNode()
        {
            var topLevelNode = _Node.GetTopLevelNode();
            Assert.IsNotNull(topLevelNode);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxNode_GetTransitionByID()
        {
            var task = _Node.GetTask(ArcFM.Process.WorkflowManager.Tasks.OpenDesign) as IMMPxTask2;
           Assert.IsNotNull(task);

           var tansition = _Node.GetTransition(task.Transition.TransitionID);
            Assert.AreEqual(tansition, task.Transition);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void IMMPxNode_GetTransitionByName()
        {
            var task = _Node.GetTask(ArcFM.Process.WorkflowManager.Tasks.OpenDesign) as IMMPxTask2;
            Assert.IsNotNull(task);

            var tansition = _Node.GetTransition(task.Transition.Name);
            Assert.AreEqual(tansition, task.Transition);
        }

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            DataTable table = base.PxApplication.ExecuteQuery("SELECT ID FROM " + base.PxApplication.GetQualifiedTableName(ArcFM.Process.WorkflowManager.Tables.Design));
            if (table.Rows.Count > 0)
            {
                int nodeID = table.Rows[0].Field<int>(0);
                using (Design design = new Design(base.PxApplication))
                {
                    design.Initialize(nodeID);

                    _Node = design.Node;
                }
            }
        }

        #endregion
    }
}