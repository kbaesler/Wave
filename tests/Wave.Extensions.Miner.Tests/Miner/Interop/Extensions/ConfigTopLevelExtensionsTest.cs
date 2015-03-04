using System;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Geodatabase;
using Miner.Interop;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class ConfigTopLevelExtensionsTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        public void IMMConfigTopLevel_ChangeVisibility()
        {
            IFeatureClass testClass = base.GetTestClass();
            Assert.IsNotNull(testClass);

            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            Assert.IsNotNull(configTopLevel);

            ISubtypes subtypes = (ISubtypes) testClass;
            IMMFieldManager fieldManager = testClass.GetFieldManager(subtypes.DefaultSubtypeCode);
            IMMFieldAdapter fieldAdapter = fieldManager.FieldByName(testClass.OIDFieldName);

            configTopLevel.ChangeVisibility(testClass, subtypes.DefaultSubtypeCode, false, testClass.OIDFieldName);
           
            Assert.IsFalse(fieldAdapter.Visible);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void IMMConfigTopLevel_ChangeVisibility_ArgumentNullException_FieldName()
        {
            IFeatureClass testClass = base.GetTestClass();
            Assert.IsNotNull(testClass);

            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            Assert.IsNotNull(configTopLevel);

            configTopLevel.ChangeVisibility(testClass, false, null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void IMMConfigTopLevel_ChangeVisibility_ArgumentNullException_Table()
        {
            IMMConfigTopLevel configTopLevel = ConfigTopLevel.Instance;
            Assert.IsNotNull(configTopLevel);

            configTopLevel.ChangeVisibility(null, false, "OBJECTID");
        }

        [TestMethod]
        public void IMMConfigTopLevel_GetAutoValues_NotNull()
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