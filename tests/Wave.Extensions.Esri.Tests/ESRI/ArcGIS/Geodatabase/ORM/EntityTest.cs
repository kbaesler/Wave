using System;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wave.Extensions.Esri.Tests.Properties;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class EntityTest : EsriTests
    {
        #region Constructors

        public EntityTest()
            : base(Settings.Default.Roadways)
        {
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void Entity_Map_Centerlines_Any()
        {
            var testClass = this.GetFeatureClass("Centerline");
            var centerlines = testClass.Map<Centerline>(new QueryFilterClass() {WhereClause = "OBJECTID < 10"});
            Assert.IsTrue(centerlines.Any());
        }

        [TestMethod]
        public void Entity_Map_Crashes_Any()
        {
            var testClass = this.GetFeatureClass("Crashes");
            var centerlines = testClass.Map<Crash>(new QueryFilterClass() {WhereClause = "OBJECTID < 10"});
            Assert.IsTrue(centerlines.Any());
        }

        #endregion
    }

    public class Crash : Entity
    {
        #region Public Properties

        [EntityField("MEASURE")]
        public double Measure { get; set; }

        [EntityField("ROUTEID")]
        public string RouteId { get; set; }

        #endregion
    }

    public class Centerline : Entity<IPolyline>
    {
        #region Public Properties

        [EntityField("FROMDATE")]
        public DateTime? FromDate { get; set; }

        [EntityField("ROADWAYIDGUID")]
        public string RoadwayGUID { get; set; }

        [EntityField("TODATE")]
        public DateTime? ToDate { get; set; }

        #endregion
    }
}