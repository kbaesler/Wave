using System.ServiceModel;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Framework;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{
    /// <summary>
    ///     A service contract for searching the active map session for table(s), class(es) and relationship(s).
    /// </summary>
    [ServiceContract]
    public interface IMapSearchService : ISearchableService<MapSearchServiceRequest>
    {

    }

    /// <summary>
    ///     The map-based search service for querying a set of table(s), class(es) and relationship(s) using the data in the
    ///     active session, and additional the matching rows are linked to a feature so that it can be located on a map.
    ///     When tables are searched, matches will be linked to the feature that participates in a relationship with the
    ///     matched non-spatial row.
    /// </summary>
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public sealed class MapSearchService : SearchableService<MapSearchServiceRequest>, IMapSearchService
    {
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
                case MapSearchServiceExtent.WithinCurrent:
                    if (relOp.Within(feature.Shape))
                        base.Add(row, layer, request);

                    break;

                case MapSearchServiceExtent.WithinCurrentOrOverlapping:
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

    /// <summary>
    ///     The requests that are issued to the searchable service.
    /// </summary>
    public class MapSearchServiceRequest : SearchableRequest
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapSearchServiceRequest" /> class.
        /// </summary>
        public MapSearchServiceRequest()
        {
            this.Extent = MapSearchServiceExtent.Any;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the extent.
        /// </summary>
        /// <value>
        ///     The extent.
        /// </value>
        public MapSearchServiceExtent Extent { get; set; }

        #endregion
    }

    /// <summary>
    ///     An enumeration of the searchable extents.
    /// </summary>
    public enum MapSearchServiceExtent
    {
        /// <summary>
        ///     Any extent.
        /// </summary>
        Any = 0,

        /// <summary>
        ///     The within the current extent.
        /// </summary>
        WithinCurrent = 1,

        /// <summary>
        ///     The within current or overlapping extents.
        /// </summary>
        WithinCurrentOrOverlapping = 2
    }
}