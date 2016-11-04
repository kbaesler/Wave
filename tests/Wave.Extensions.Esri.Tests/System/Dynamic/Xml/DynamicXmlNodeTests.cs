using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Dynamic.Xml.Tests
{
    [TestClass]
    public class DynamicXmlNodeTests
    {
        #region Public Methods

        [TestMethod]
        public void DynamicXmlNodeTests_Attribute_Exposed_As_Member()
        {
            var xdoc = XDocument.Parse("<node attr='dynamic'></node>");
            dynamic node = xdoc.Root.ToDynamic();

            Assert.AreEqual("dynamic", node.attr);
        }

        [TestMethod]
        public void DynamicXmlNodeTests_Elements_Exposed_As_Members()
        {
            var xdoc = XDocument.Parse("<node><child>caden</child></node>");
            dynamic node = xdoc.Root.ToDynamic();

            Assert.AreEqual("caden", node.child);
        }

        [TestMethod]
        public void DynamicXmlNodeTests_Pluralized_Children_Via_Pluralized_Word()
        {
            var xdoc = XDocument.Parse("<node><other/><other/><other/></node>");
            dynamic node = xdoc.Root.ToDynamic();

            var others = node.others;

            Assert.IsNotNull(others);
            Assert.IsInstanceOfType(others, typeof (DynamicXmlNodeList));
            Assert.AreEqual(3, others.Length);
        }

        #endregion
    }
}