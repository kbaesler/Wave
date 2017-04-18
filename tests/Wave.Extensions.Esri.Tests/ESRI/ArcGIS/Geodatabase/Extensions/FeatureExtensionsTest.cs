using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class FeatureExtensionsTest : RoadwaysTests
    {
        #region Public Methods

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeature_GetDifferences_IsNotNull()
        {
            var testClass = base.GetPointFeatureClass();
            Assert.IsNotNull(testClass);

            var feature = testClass.Fetch(1);
            Assert.IsNotNull(feature);

            IPoint point = feature.ShapeCopy as IPoint;
            if (point == null) return;

            point.X += 10;
            point.Y += 10;

            feature.Shape = point;

            var shape = feature.GetDifference();
            Assert.IsNotNull(shape);
        }

        [TestMethod]
        [TestCategory("ESRI")]
        public void IFeature_GetDifferences_IsNull()
        {
            var testClass = base.GetPointFeatureClass();
            Assert.IsNotNull(testClass);

            var feature = testClass.Fetch(1);
            Assert.IsNotNull(feature);

            var shape = feature.GetDifference();
            Assert.IsNull(shape);
        }

        #endregion
    }
}