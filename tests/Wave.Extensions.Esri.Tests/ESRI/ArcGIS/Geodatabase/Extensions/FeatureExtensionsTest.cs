using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class FeatureExtensionsTest : EsriTests
    {
        #region Public Methods

        [TestMethod]
        public void IFeature_GetDifferences_IsNotNull()
        {
            var testClass = base.GetTestClass();
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
        public void IFeature_GetDifferences_IsNull()
        {
            var testClass = base.GetTestClass();
            Assert.IsNotNull(testClass);

            var feature = testClass.Fetch(1);
            Assert.IsNotNull(feature);

            var shape = feature.GetDifference();
            Assert.IsNull(shape);
        }

        #endregion
    }
}