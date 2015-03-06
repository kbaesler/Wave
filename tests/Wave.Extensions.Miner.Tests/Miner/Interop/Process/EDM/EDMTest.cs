using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop.Process;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class EDMTest
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void EDM_Constructor()
        {
            EDM edm = new EDM("Name", "Value", "Type");
            Assert.IsNotNull(edm);
            Assert.IsNotNull(edm.Name);
            Assert.IsNotNull(edm.Value);
            Assert.IsNotNull(edm.Type);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void EDM_AreNotEqual()
        {
            EDM edm1 = new EDM("Name1", "Value1", "Type1");
            EDM edm2 = new EDM("Name2", "Value2", "Type2");

            Assert.AreNotEqual(edm1, edm2);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void EDM_AreEqual()
        {
            EDM edm1 = new EDM("Name", "Value", "Type");
            EDM edm2 = new EDM("Name", "Value", "Type");

            Assert.AreEqual(edm1, edm2);
        }

        #endregion
    }
}