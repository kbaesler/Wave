using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableInventoryTests
    {
        #region Public Methods

        [TestMethod]
        public void SearchableSet_ToJson()
        {
            List<SearchableInventory> sets = new List<SearchableInventory>();

            SearchableInventory set1 = new SearchableInventory("Distribution Equipment",
                new SearchableTable("NonControllableGasValve",
                    new SearchableField("Operator", "Consultant"),
                    new SearchableField("PDLSNumber")),
                new SearchableTable("NonControllableGasValve",
                    new SearchableField("Operator", "Consultant"),
                    new SearchableField("ValveTag2"),
                    new SearchableField("ValveTag1")),
                new SearchableTable("HPPipe",
                    new SearchableField("Operator", "Consultant"),
                    new SearchableField("LineNumber")));

            SearchableInventory set2 = new SearchableInventory("Work History",
                new SearchableTable("GasWorkHistory",
                    new[] {new SearchableField("WorkOrderNum")}, new SearchableRelationship()));

            sets.Add(set1);
            sets.Add(set2);

            var json = JsonConvert.SerializeObject(sets);
        }

        #endregion
    }
}