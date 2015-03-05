using System.Linq;

using ESRI.ArcGIS.esriSystem;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class PropertySetExtensionTest : EsriTests
    {
        #region Fields

        private IPropertySet _PropertySet;

        #endregion

        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IPropertySet_AsEnumerable_Count_Equals_3()
        {
            Assert.AreEqual(3, _PropertySet.AsEnumerable().Count());
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IPropertySet_GetProperty_Equals_Type()
        {
            Assert.AreEqual(".NET", _PropertySet.GetProperty("String", string.Empty));
            Assert.AreEqual(1, _PropertySet.GetProperty("Int32", -1));
            Assert.AreEqual(2.0, _PropertySet.GetProperty("Double", -1.0));
            Assert.AreEqual(null, _PropertySet.GetProperty<object>("NULL", null));
        }

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _PropertySet = new PropertySetClass();
            _PropertySet.SetProperty("String", ".NET");
            _PropertySet.SetProperty("Int32", 1);
            _PropertySet.SetProperty("Double", 2.0);
        }

        #endregion
    }
}