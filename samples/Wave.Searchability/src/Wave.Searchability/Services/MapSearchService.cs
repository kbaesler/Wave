using System.Collections.Generic;
using System.ServiceModel;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Framework;
using Miner.Framework.Search;
using Miner.Interop;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{
    [ServiceContract]
    public interface IMapSearchService
    {
        #region Public Methods

        [OperationContract]
        SearchableResponse Find(MapSearchServiceRequest request);

        [OperationContract]
        SearchableResponse Find(string keywords, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, IEnumerable<SearchableSet> sets, int threshold, MapSearchableExtent extent);

        #endregion
    }

    public sealed class MapSearchService : SearchableService<MapSearchServiceRequest>, IMapSearchService
    {
        #region IMapSearchService Members

        public SearchableResponse Find(string keywords, ComparisonOperator comparisonOperator, LogicalOperator logicalOperator, IEnumerable<SearchableSet> sets, int threshold, MapSearchableExtent extent)
        {            
            return base.Find(new MapSearchServiceRequest
            {
                ComparisonOperator = comparisonOperator,
                Keywords = keywords,
                LogicalOperator = logicalOperator,
                Items = sets,
                Threshold = threshold,
                Extent = extent
            });
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="request">The request.</param>
        protected override void Add(IRow row, IFeatureLayer layer, MapSearchServiceRequest request)
        {
            var feature = (IFeature) row;
            var relOp = (IRelationalOperator) Document.ActiveView.Extent.Envelope;

            switch (request.Extent)
            {
                case MapSearchableExtent.WithinCurrent:
                    if (relOp.Within(feature.Shape))
                        base.Add(row, layer, request);

                    break;

                case MapSearchableExtent.WithinCurrentOrOverlapping:
                    if (relOp.Within(feature.Shape) || relOp.Overlaps(feature.Shape))
                        base.Add(row, layer, request);

                    break;

                default:
                    base.Add(row, layer, request);
                    break;
            }
        }

        #endregion
    }


    public class MapSearchServiceRequest : SearchableRequest
    {
        #region Public Properties

        public MapSearchableExtent Extent { get; set; }

        #endregion
    }

    public enum MapSearchableExtent
    {
        Any = 0,
        WithinCurrent = 1,
        WithinCurrentOrOverlapping = 2
    }
}