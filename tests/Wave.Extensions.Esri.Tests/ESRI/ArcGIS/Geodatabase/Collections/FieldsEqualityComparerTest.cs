using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class FieldsEqualityComparerTest : RoadwaysTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void FieldsEqualityComparer_Equals_False()
        {
            FieldsEqualityComparer comparer = new FieldsEqualityComparer();

            var testTable = base.GetTable();
            var rows = testTable.Fetch(1, 2);

            Assert.AreEqual(2, rows.Count);

            var equals = comparer.Equals(rows.First(), rows.Last());
            Assert.IsFalse(equals);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void FieldsEqualityComparer_Equals_True()
        {
            FieldsEqualityComparer comparer = new FieldsEqualityComparer();

            var testTable = base.GetTable();
            var row = testTable.Fetch(1);

            Assert.IsNotNull(row);

            var equals = comparer.Equals(row, row);
            Assert.IsTrue(equals);
        }

        #endregion
    }
}