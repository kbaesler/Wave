using System;

using ESRI.ArcGIS.Geodatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;
using Wave.Searchability.Services;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableRequestTests
    {
        [TestMethod]
        public void TextSearchServiceRequest_ToJson()
        {
            TextSearchServiceRequest request = new TextSearchServiceRequest();
            request.ComparisonOperator = ComparisonOperator.Equals;
            request.Keyword = "kellyl";
            request.LogicalOperator = LogicalOperator.Or;
            request.Threshold = 200;
            request.Inventory.Add(new SearchableInventory("Layers", new SearchableLayer("NonControllableGasValve", new SearchableField("CreationUser"))));
            request.Inventory.Add(new SearchableInventory("Tables", new SearchableTable("TransformerUnit", new SearchableField("CreationUser"))));

            var json = JsonConvert.SerializeObject(request);
            Assert.AreEqual("{\"comparisonOperator\":0,\"inventory\":[{\"items\":[{\"layerDefinition\":false,\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"NonControllableGasValve\"}],\"name\":\"Layers\"},{\"items\":[{\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"TransformerUnit\"}],\"name\":\"Tables\"}],\"keyword\":\"kellyl\",\"logicalOperator\":1,\"threshold\":200}", json);
        }

        [TestMethod]
        public void MapSearchServiceRequest_ToJson()
        {
            MapSearchServiceRequest request = new MapSearchServiceRequest();
            request.Extent = MapSearchServiceExtent.WithinCurrentExtent;
            request.ComparisonOperator = ComparisonOperator.Equals;
            request.Keyword = "kellyl";
            request.LogicalOperator = LogicalOperator.Or;
            request.Threshold = 200;
            request.Inventory.Add(new SearchableInventory("Layers", new SearchableLayer("NonControllableGasValve", new SearchableField("CreationUser"))));
            request.Inventory.Add(new SearchableInventory("Tables", new SearchableTable("TransformerUnit", new SearchableField("CreationUser"))));

            var json = JsonConvert.SerializeObject(request);
            Assert.AreEqual("{\"extent\":1,\"comparisonOperator\":0,\"inventory\":[{\"items\":[{\"layerDefinition\":false,\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"NonControllableGasValve\"}],\"name\":\"Layers\"},{\"items\":[{\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"TransformerUnit\"}],\"name\":\"Tables\"}],\"keyword\":\"kellyl\",\"logicalOperator\":1,\"threshold\":200}", json);
        }
    }
}
