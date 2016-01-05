using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Wave.Searchability.Data;

namespace Wave.Searchability.Tests.Data.Model
{
    [TestClass]
    public class SearchableSetTests
    {
        #region Public Methods

        [TestMethod]
        public void SearchableSet_ToJson()
        {
            List<SearchableSet> sets = new List<SearchableSet>();

            SearchableSet set1 = new SearchableSet("Distribution Equipment",
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

            SearchableSet set2 = new SearchableSet("Work History",
                new SearchableTable("GasWorkHistory",
                    new[] {new SearchableField("WorkOrderNum")}, new SearchableRelationship()));

            sets.Add(set1);
            sets.Add(set2);

            var json = JsonConvert.SerializeObject(sets);
        }

        #endregion
    }
}