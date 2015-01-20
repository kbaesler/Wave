using System;

using ESRI.ArcGIS.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests.ESRI.ArcGIS.Geometry.Collections
{
    [TestClass]
    public class PolylineEqualityComparerTest
    {
        [TestMethod]
        public void PolylineEqualityComparer_Equals_False()
        {
            IPoint fromPoint = new PointClass();
            fromPoint.PutCoords(1, 3);

            IPoint toPoint = new PointClass();
            toPoint.PutCoords(3, 1);

            ILine line = new LineClass();
            line.PutCoords(fromPoint, toPoint);

            PolylineEqualityComparer comparer = new PolylineEqualityComparer(.01);
            Assert.IsFalse(comparer.Equals(line, null));
        }

        [TestMethod]
        public void PolylineEqualityComparer_Equals_True()
        {
            IPoint fromPoint = new PointClass();
            fromPoint.PutCoords(1, 3);

            IPoint toPoint = new PointClass();
            toPoint.PutCoords(3, 1);

            ILine line = new LineClass();
            line.PutCoords(fromPoint, toPoint);

            PolylineEqualityComparer comparer = new PolylineEqualityComparer(.01);
            Assert.IsTrue(comparer.Equals(line, line));
        }
    }
}
