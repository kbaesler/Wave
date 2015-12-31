using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableTableTests
    {
        #region Public Methods

        [TestMethod]
        public void SearchableTable_Simple()
        {
            var table = new SearchableTable("NonControllableGasValve",
                new SearchableField("Operator", "Consultant"),
                new SearchableField("PDLSNumber"));

            Assert.AreEqual(table.Name, "NonControllableGasValve");
            Assert.IsTrue(table.Fields.Any());

            var json = JsonConvert.SerializeObject(table);
            Assert.AreEqual("{\"isFeatureClass\":false,\"layerDefinition\":false,\"nameAsClassModelName\":false,\"fields\":[{\"value\":\"Consultant\",\"visible\":false,\"name\":\"Operator\"},{\"value\":null,\"visible\":false,\"name\":\"PDLSNumber\"}],\"relationships\":[],\"name\":\"NonControllableGasValve\"}", json);
        }

        #endregion
    }
}