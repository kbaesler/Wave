using System;
using System.Linq;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wave.Extensions.Esri.Tests
{
    [TestClass]
    public class SelectionSetExtensionsTest : EsriTests
    {
        [TestMethod]
        public void ISelectionSet_Remove_AreEqual()
        {
            var map = this.CreateMap();
            var layer = map.Where<IFeatureLayer>(l => l.Valid).FirstOrDefault();
            Assert.IsNotNull(layer);

            IQueryFilter filter = new QueryFilterClass { WhereClause = string.Format("{0} < 1000", layer.FeatureClass.OIDFieldName) };
            IFeatureSelection featureSelection = (IFeatureSelection) layer;                       
            featureSelection.SelectFeatures(filter, esriSelectionResultEnum.esriSelectionResultNew, false);

            ISelectionSet selectionSet = featureSelection.SelectionSet;
            var oids = selectionSet.IDs.AsEnumerable().ToArray();
            selectionSet.Remove(oids);

            Assert.AreEqual(0, selectionSet.Count);
        }
    }
}
