using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Framework.Dispatch;
using Miner.Interop;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class AutoUpdaterModeReverterTest
    {
        #region Public Methods

        [TestMethod]
        public void AutoUpdaterModeReverter_Equals_NoEvents()
        {
            using (new AutoUpdaterModeReverter(mmAutoUpdaterMode.mmAUMNoEvents))
            {
                IMMAutoUpdater o = new MMAutoupdaterDispatch();
                Assert.AreEqual(mmAutoUpdaterMode.mmAUMNoEvents, o.AutoUpdaterMode);
            }
        }

        #endregion
    }
}