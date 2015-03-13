using System.Linq;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class CursorExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void ICursor_Batch_ToDictionary_IsTrue()
        {
            var table = base.GetTestTable();

            using (ComReleaser cr = new ComReleaser())
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = table.OIDFieldName + "< 10";

                ICursor cursor = table.Search(filter, false);
                cr.ManageLifetime(cursor);

                var batches = cursor.Batch<int>(table.OIDFieldName, 2).ToArray();
                Assert.AreEqual(batches.Count(), 5);

                int count = batches.Sum(batch => batch.Count);
                Assert.IsTrue(count < 10);
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ICursor_Batch_ToList_IsTrue()
        {
            var table = base.GetTestTable();

            using (ComReleaser cr = new ComReleaser())
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = table.OIDFieldName + "< 10";

                ICursor cursor = table.Search(filter, false);
                cr.ManageLifetime(cursor);

                var batches = cursor.Batch(2).ToList();
                Assert.AreEqual(batches.Count(), 5);

                int count = batches.Sum(batch => batch.Count());
                Assert.IsTrue(count < 10);
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ICursor_GetXDocument_Any_Rows()
        {
            var table = base.GetTestTable();

            using (ComReleaser cr = new ComReleaser())
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = table.OIDFieldName + "< 10";

                ICursor cursor = table.Search(filter, true);
                cr.ManageLifetime(cursor);

                var xdoc = cursor.GetXDocument();
                Assert.IsTrue(xdoc.Root.DescendantNodes().Any());
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ICursor_GetXDocument_NotNull()
        {
            var table = base.GetTestTable();

            using (ComReleaser cr = new ComReleaser())
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = table.OIDFieldName + "< 1";

                ICursor cursor = table.Search(filter, true);
                cr.ManageLifetime(cursor);

                var xdoc = cursor.GetXDocument();
                Assert.IsNotNull(xdoc);
            }
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void ICursor_GetXDocument_Root_Name_Equal_Test()
        {
            var table = base.GetTestTable();

            using (ComReleaser cr = new ComReleaser())
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = table.OIDFieldName + "< 1";

                ICursor cursor = table.Search(filter, true);
                cr.ManageLifetime(cursor);

                var xdoc = cursor.GetXDocument("Test");
                Assert.AreEqual("Test", xdoc.Root.Name);
            }
        }

        #endregion
    }
}