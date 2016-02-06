using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;
using System.Collections.Concurrent;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableResponseTests
    {
        [TestMethod]
        public void SearchableResponse_ToJson()
        {
            SearchableResponse response = new SearchableResponse();
            response.Add("ControllableGasValve", new List<int>(new [] {1, 2, 3, 4, 5}));
            response.Add("NonControllableGasValve", new List<int>(new [] { 125, 2888 }));

            var json = JsonConvert.SerializeObject(response);
            Assert.AreEqual("{\"ControllableGasValve\":[1,2,3,4,5],\"NonControllableGasValve\":[125,2888]}", json);
        }
    }
}
