using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Geodatabase;
using Miner.Interop;
using Miner.Interop.Extensions;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class ConfigTopLevelExtensionsTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        public void IMMConfigTopLevel_GetAutoValues()
        {
            IFeatureClass testClass = base.GetTestClass();
            Assert.IsNotNull(testClass);

            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            Assert.IsNotNull(configTopLevel);

            var list = configTopLevel.GetAutoValues(testClass, mmEditEvent.mmEventFeatureCreate);
            Assert.IsTrue(list.Count > 0);
        }

        #endregion
    }
}