using ESRI.ArcGIS.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class LineExtensionsTest : EsriTests
    {
        #region Public Methods

        /// <summary>
        ///     Gets the arithmetic angle.
        /// </summary>
        [TestMethod]
        public void ILine_GetArithmeticAngle_Equal_315()
        {
            ILine line = this.CreateLine();
            Assert.IsNotNull(line);

            double angle = line.GetArithmeticAngle();
            Assert.AreEqual(315.0, angle);
        }

        /// <summary>
        ///     Gets the geographic angle.
        /// </summary>
        [TestMethod]
        public void ILine_GetGeographicAngle_Equal_90()
        {
            ILine line = this.CreateLine();
            Assert.IsNotNull(line);

            double angle = line.GetGeographicAngle();
            Assert.AreEqual(90.785398163397446, angle);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the line.
        /// </summary>
        /// <returns></returns>
        private ILine CreateLine()
        {
            IPoint fromPoint = new PointClass();
            fromPoint.PutCoords(1, 3);

            IPoint toPoint = new PointClass();
            toPoint.PutCoords(3, 1);

            ILine line = new LineClass();
            line.PutCoords(fromPoint, toPoint);

            return line;
        }

        #endregion
    }
}