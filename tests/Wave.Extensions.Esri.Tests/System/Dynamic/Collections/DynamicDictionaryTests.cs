using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Dynamic.Collections.Tests
{
    [TestClass]
    public class DynamicDictionaryTests
    {
        #region Public Methods

        [TestMethod]
        public void DynamicDictionaryTests_Key_Maps_To_Property()
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            items.Add("hello", "world");

            dynamic d = items.ToDynamic();

            Assert.AreEqual(items["hello"], d.hello);
        }

        [TestMethod]
        public void DynamicDictionaryTests_New_Key_Added_Via_Property()
        {
            Dictionary<string, string> items = new Dictionary<string, string>();

            dynamic d = items.ToDynamic();
            d.hello = "world";

            Assert.AreEqual("world", d.hello);
        }

        #endregion
    }
}