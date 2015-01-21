using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class TreeViewSelectionEnumeratorTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        public void TreeViewSelectionEnumerator_EOF_IsTrue()
        {
            IFeatureClass testClass = base.Workspace.GetFeatureClass("TRANSFORMER");
            Assert.IsNotNull(testClass);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = "OBJECTID < 10";

            var list = testClass.Fetch(filter);
            var enumerator = new TreeViewSelectionEnumerator(list);

            while ((enumerator.Next) != null)
            {
            }

            Assert.IsTrue(enumerator.EOF);
        }

        [TestMethod]
        public void TreeViewSelectionEnumerator_IsEmpty_IsTrue()
        {
            var enumerator = new TreeViewSelectionEnumerator();
            Assert.IsTrue(enumerator.EOF);
        }

        [TestMethod]
        public void TreeViewSelectionEnumerator_Next_IsTrue()
        {
            IFeatureClass testClass = base.Workspace.GetFeatureClass("TRANSFORMER");
            Assert.IsNotNull(testClass);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = "OBJECTID < 10";

            var list = testClass.Fetch(filter);
            var enumerator = new TreeViewSelectionEnumerator(list);

            ID8ListItem item = enumerator.Next;
            Assert.IsTrue(item is ID8Feature);
        }

        [TestMethod]
        public void TreeViewSelectionEnumerator_Reset_Equal()
        {
            IFeatureClass testClass = base.Workspace.GetFeatureClass("TRANSFORMER");
            Assert.IsNotNull(testClass);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = "OBJECTID < 10";

            var list = testClass.Fetch(filter);
            var enumerator = new TreeViewSelectionEnumerator(list);

            ID8GeoAssoc x = enumerator.Next as ID8GeoAssoc;
            Assert.IsNotNull(x);

            enumerator.Reset();

            ID8GeoAssoc y = enumerator.Next as ID8GeoAssoc;
            Assert.IsNotNull(y);

            Assert.AreEqual(x.TableName, y.TableName);
            Assert.AreEqual(x.OID, y.OID);
        }

        #endregion
    }
}