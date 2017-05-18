using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Wave.Searchability.Services
{
    /// <summary>
    ///     A service contract for searching the active map session for table(s), class(es) and relationship(s).
    /// </summary>
    [ServiceContract]
    public interface IMapSearchService : ITextSearchService<MapSearchServiceRequest>
    {
    }

    /// <summary>
    ///     The map-based search service for querying a set of table(s), class(es) and relationship(s) using the data in the
    ///     active session, and additional the matching rows are linked to a feature so that it can be located on a map.
    ///     When tables are searched, matches will be linked to the feature that participates in a relationship with the
    ///     matched non-spatial row.
    /// </summary>
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public sealed class MapSearchService : TextSearchService<MapSearchServiceRequest>, IMapSearchService
    {
        #region Protected Methods

        /// <summary>
        ///     Adds the specified feature to the response.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="layer">The feature layer for the feature.</param>
        /// <param name="request">The request.</param>
        /// <param name="map">The map.</param>
        /// <param name="token">The token.</param>
        protected override void Add(IFeature feature, IFeatureLayer layer, MapSearchServiceRequest request, IMap map, CancellationToken token)
        {
            if (request.Extent != MapSearchServiceExtent.WithinAnyExtent)
            {
                var geometry = feature.ShapeCopy;
                geometry.Project(map.SpatialReference);

                var extent = ((IActiveView) map).Extent;
                var operators = (IRelationalOperator) geometry;

                var within = operators.Within(extent);
                if (!within && request.Extent == MapSearchServiceExtent.WithinOrOverlappingCurrentExtent)
                {
                    if (!operators.Overlaps(extent))
                        return;
                }
                else if (!within)
                {
                    return;
                }
            }

            base.Add(feature, layer, request, map, token);
        }

        #endregion
    }

    /// <summary>
    ///     The requests that are issued to the searchable service.
    /// </summary>
    [DataContract(Name = "request")]
    public class MapSearchServiceRequest : TextSearchServiceRequest
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapSearchServiceRequest" /> class.
        /// </summary>
        public MapSearchServiceRequest()
        {
            this.Extent = MapSearchServiceExtent.WithinCurrentExtent;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the extent.
        /// </summary>
        /// <value>
        ///     The extent.
        /// </value>
        [DataMember(Name = "extent")]
        public MapSearchServiceExtent Extent { get; set; }

        #endregion
    }

    /// <summary>
    ///     An enumeration of the searchable extents.
    /// </summary>
    public enum MapSearchServiceExtent
    {
        /// <summary>
        ///     Within any extent.
        /// </summary>
        WithinAnyExtent = 0,

        /// <summary>
        ///     Within the current extent.
        /// </summary>
        WithinCurrentExtent = 1,

        /// <summary>
        ///     Within or overlapping current extent
        /// </summary>
        WithinOrOverlappingCurrentExtent = 2
    }
}