using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableFieldTests
    {
        #region Public Methods

        [TestMethod]
        public void SearchableField_Simple()
        {
            var item = new SearchableField("Operator", "Consultant");
            Assert.AreEqual("Operator", item.Name);
            Assert.AreEqual("Consultant", item.Value);
            Assert.AreEqual(false, item.Visible);

            var json = JsonConvert.SerializeObject(item);
            Assert.AreEqual("{\"value\":\"Consultant\",\"visible\":false,\"name\":\"Operator\"}", json);
        }

        #endregion
    }
}