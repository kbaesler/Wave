using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class RowEqualityComparerTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void RowEqualityComparer_Equals_False()
        {
            RowEqualityComparer comparer = new RowEqualityComparer();

            var testTable = base.GetTestTable();
            var rows = testTable.Fetch(1, 2);

            Assert.AreEqual(2, rows.Count);

            var equals = comparer.Equals(rows.First(), rows.Last());
            Assert.IsFalse(equals);
        }

        [TestMethod]
        public void RowEqualityComparer_Equals_True()
        {
            RowEqualityComparer comparer = new RowEqualityComparer();

            var testTable = base.GetTestTable();
            var row = testTable.Fetch(1);

            Assert.IsNotNull(row);

            var equals = comparer.Equals(row, row);
            Assert.IsTrue(equals);
        }

        #endregion
    }
}