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
            request.Items.Add(new SearchableLayer("NonControllableGasValve", new SearchableField("CreationUser")));
            request.Items.Add(new SearchableTable("TransformerUnit", new SearchableField("CreationUser")));

            var json = JsonConvert.SerializeObject(request);
            Assert.AreEqual("{\"comparisonOperator\":0,\"items\":[{\"layerDefinition\":false,\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"NonControllableGasValve\"},{\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"TransformerUnit\"}],\"keyword\":\"kellyl\",\"logicalOperator\":1,\"threshold\":200}", json);
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
            request.Items.Add(new SearchableLayer("NonControllableGasValve", new SearchableField("CreationUser")));
            request.Items.Add(new SearchableTable("TransformerUnit", new SearchableField("CreationUser")));

            var json = JsonConvert.SerializeObject(request);
            Assert.AreEqual("{\"extent\":1,\"comparisonOperator\":0,\"items\":[{\"layerDefinition\":false,\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"NonControllableGasValve\"},{\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"CreationUser\"}],\"relationships\":[],\"name\":\"TransformerUnit\"}],\"keyword\":\"kellyl\",\"logicalOperator\":1,\"threshold\":200}", json);
        }
    }
}
