using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop.Process.Collections;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class ReferenceDictionaryTest
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("Miner")]
        public void ReferenceDictionary_Count()
        {
            var dict = new ReferenceDictionary();
            dict.Add("A", 1);
            dict.Add("B", 2);
            dict.Add("C", 3);

            Assert.IsTrue(dict.Count == 3);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ReferenceDictionary_Indexer()
        {
            var dict = new ReferenceDictionary();
            dict.Add("A", 1);
            dict.Add("B", 2);
            dict.Add("C", 3);

            Assert.AreEqual(dict["B"], 2);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ReferenceDictionary_Keys()
        {
            var dict = new ReferenceDictionary();
            dict.Add("A", 1);
            dict.Add("B", 2);
            dict.Add("C", 3);

            Assert.AreEqual(dict.Keys.Count, 3);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ReferenceDictionary_Remove()
        {
            var dict = new ReferenceDictionary();
            dict.Add("A", 1);
            dict.Add("B", 2);
            dict.Add("C", 3);
            dict.Remove("C");

            Assert.AreEqual(dict.Count, 2);
        }

        [TestMethod]
        [TestCategory("Miner")]
        public void ReferenceDictionary_Values()
        {
            var dict = new ReferenceDictionary();
            dict.Add("A", 1);
            dict.Add("B", 2);
            dict.Add("C", 3);

            Assert.AreEqual(dict.Values.Count, 3);
        }

        #endregion
    }
}