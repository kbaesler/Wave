using ESRI.ArcGIS.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class GeometryEqualityComparerTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void GeometryEqualityComparer_Equals_False()
        {
            IPoint fromPoint = new PointClass();
            fromPoint.PutCoords(1, 3);

            IPoint toPoint = new PointClass();
            toPoint.PutCoords(3, 1);

            ILine line = new LineClass();
            line.PutCoords(fromPoint, toPoint);

            GeometryEqualityComparer comparer = new GeometryEqualityComparer();
            Assert.IsFalse(comparer.Equals(line, null));
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void GeometryEqualityComparer_Equals_True()
        {
            IPoint fromPoint = new PointClass();
            fromPoint.PutCoords(1, 3);

            IPoint toPoint = new PointClass();
            toPoint.PutCoords(3, 1);

            ILine line = new LineClass();
            line.PutCoords(fromPoint, toPoint);

            GeometryEqualityComparer comparer = new GeometryEqualityComparer();
            Assert.IsTrue(comparer.Equals(line, line));
        }

        #endregion
    }
}