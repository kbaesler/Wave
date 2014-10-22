using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Miner.Interop;

namespace Wave.Extensions.Miner.Tests
{
    [TestClass]
    public class TreeViewCollectionTest : MinerTests
    {
        #region Public Methods

        [TestMethod]
        public void TreeViewCollection_EOF_IsTrue()
        {
            IFeatureClass testClass = base.Workspace.GetFeatureClass("TRANSFORMER");
            Assert.IsNotNull(testClass);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = "OBJECTID < 10";

            var list = testClass.Fetch(filter);
            TreeViewCollection collection = new TreeViewCollection(list);

            while ((collection.Next) != null)
            {
            }

            Assert.IsTrue(collection.EOF);
        }

        [TestMethod]
        public void TreeViewCollection_IsEmpty_IsTrue()
        {
            TreeViewCollection collection = new TreeViewCollection();
            Assert.IsTrue(collection.EOF);
        }

        [TestMethod]
        public void TreeViewCollection_Next_IsTrue()
        {
            IFeatureClass testClass = base.Workspace.GetFeatureClass("TRANSFORMER");
            Assert.IsNotNull(testClass);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = "OBJECTID < 10";

            var list = testClass.Fetch(filter);
            TreeViewCollection collection = new TreeViewCollection(list);

            ID8ListItem item = collection.Next;
            Assert.IsTrue(item is ID8Feature);
        }

        [TestMethod]
        public void TreeViewCollection_Reset_Equal()
        {
            IFeatureClass testClass = base.Workspace.GetFeatureClass("TRANSFORMER");
            Assert.IsNotNull(testClass);

            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = "OBJECTID < 10";

            var list = testClass.Fetch(filter);
            TreeViewCollection collection = new TreeViewCollection(list);

            ID8GeoAssoc x = collection.Next as ID8GeoAssoc;
            Assert.IsNotNull(x);

            collection.Reset();

            ID8GeoAssoc y = collection.Next as ID8GeoAssoc;
            Assert.IsNotNull(y);

            Assert.AreEqual(x.TableName, y.TableName);
            Assert.AreEqual(x.OID, y.OID);
        }

        #endregion
    }
}