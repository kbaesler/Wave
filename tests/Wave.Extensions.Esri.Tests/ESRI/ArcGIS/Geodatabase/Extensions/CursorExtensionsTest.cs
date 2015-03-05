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