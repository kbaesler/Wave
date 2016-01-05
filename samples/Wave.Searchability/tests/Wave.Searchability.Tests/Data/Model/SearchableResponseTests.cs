using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableResponseTests
    {
        [TestMethod]
        public void SearchableResponse_Init()
        {
            SearchableResponse response = new SearchableResponse();
            response.Add("FeatureClassName", new List<int>(new int[] {1, 2, 3, 4, 5}));

            var json = JsonConvert.SerializeObject(response);
            Assert.AreEqual("", json);
        }
    }
}
