using ESRI.ArcGIS.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class PolylineExtensionsTest : EsriTests
    {
        #region Public Methods

        /// <summary>
        ///     Gets the angle.
        /// </summary>
        [TestMethod]
        [TestCategory("ESRI")]
        public void IPolyline_GetAngle_Negative_0()
        {
            IPolyline polyline = this.CreatePolyline();
            Assert.IsNotNull(polyline);

            double angle = polyline.GetAngle();
            Assert.AreEqual(-0.78539816339744828, angle);
        }

        /// <summary>
        ///     Gets the arithmetic angle.
        /// </summary>
        [TestMethod]
        [TestCategory("ESRI")]
        public void IPolyline_GetArithmeticAngle_Equal_315()
        {
            IPolyline polyline = this.CreatePolyline();
            Assert.IsNotNull(polyline);

            double angle = polyline.GetArithmeticAngle();
            Assert.AreEqual(315.0, angle);
        }

        /// <summary>
        ///     Gets the geographic angle.
        /// </summary>
        [TestMethod]
        [TestCategory("ESRI")]
        public void IPolyline_GetGeographicAngle_Equal_90()
        {
            IPolyline polyline = this.CreatePolyline();
            Assert.IsNotNull(polyline);

            double angle = polyline.GetGeographicAngle();
            Assert.AreEqual(90.785398163397446, angle);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the polyline.
        /// </summary>
        /// <returns></returns>
        private IPolyline CreatePolyline()
        {
            IPoint fromPoint = new PointClass();
            fromPoint.PutCoords(1, 3);

            IPoint toPoint = new PointClass();
            toPoint.PutCoords(3, 1);

            ILine line = new LineClass();
            line.PutCoords(fromPoint, toPoint);

            ISegmentCollection collection = new PolylineClass();
            collection.AddSegment((ISegment) line);

            return (IPolyline) collection;
        }

        #endregion
    }
}