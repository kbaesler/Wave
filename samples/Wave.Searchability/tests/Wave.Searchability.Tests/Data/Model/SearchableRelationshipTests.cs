using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableRelationshipTests
    {
        #region Public Methods

        [TestMethod]
        public void SeachableRelationship_Simple()
        {
            var relationship = new SearchableRelationship(new SearchableField("OBJECTID"));
            Assert.AreEqual(Searchable.Any, relationship.Name);
            Assert.IsTrue(relationship.Fields.Any());

            var json = JsonConvert.SerializeObject(relationship);
            Assert.AreEqual("{\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"OBJECTID\"}],\"relationships\":[],\"name\":\"*\"}", json);
        }

        #endregion
    }
}