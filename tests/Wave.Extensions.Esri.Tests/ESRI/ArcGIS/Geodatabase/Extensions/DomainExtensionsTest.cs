using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class DomainExtensionsTest : RoadwaysTests
    {
        #region Fields

        private ICodedValueDomain _CodedValueDomain;

        #endregion

        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IDomain_AsEnumerable_Count_Equals_4()
        {
            Assert.AreEqual(4, _CodedValueDomain.AsEnumerable().Count());
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IDomain_GetDescription_Equals_Residential()
        {
            string name = _CodedValueDomain.GetDescription("RES");
            Assert.AreEqual("Residential", name);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IDomain_GetValue_Equals_COM()
        {
            Assert.AreEqual("COM", _CodedValueDomain.GetValue("Commercial", ""));
        }

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            // The code to create a new coded value domain.
            _CodedValueDomain = new CodedValueDomainClass();

            // Value and name pairs.
            _CodedValueDomain.AddCode("RES", "Residential");
            _CodedValueDomain.AddCode("COM", "Commercial");
            _CodedValueDomain.AddCode("IND", "Industrial");
            _CodedValueDomain.AddCode("BLD", "Building");
        }

        #endregion
    }
}