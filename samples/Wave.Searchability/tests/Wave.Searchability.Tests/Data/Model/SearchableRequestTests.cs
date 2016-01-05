using System;

using ESRI.ArcGIS.Geodatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableRequestTests
    {
        [TestMethod]
        public void SearchableRequest_ToJson()
        {
            SearchableRequest request = new SearchableRequest();
            request.ComparisonOperator = ComparisonOperator.Equals;
            request.Keywords = "kellyl";
            request.LogicalOperator = LogicalOperator.Or;
            request.Threshold = 200;    
            request.Items.Add(new SearchableSet("Distribution Equipment", new SearchableTable("NonControllableGasValve", new SearchableField("Operator"))));

            var json = JsonConvert.SerializeObject(request);
            Assert.AreEqual("{\"comparisonOperator\":0,\"keywords\":\"kellyl\",\"logicalOperator\":1,\"items\":[{\"tables\":[{\"isFeatureClass\":false,\"layerDefinition\":false,\"nameAsClassModelName\":false,\"fields\":[{\"value\":null,\"visible\":false,\"name\":\"Operator\"}],\"relationships\":[],\"name\":\"NonControllableGasValve\"}],\"name\":\"Distribution Equipment\"}],\"threshold\":200}", json);
        }
    }
}
