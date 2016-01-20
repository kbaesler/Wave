using System.Runtime.Serialization;
using System.ServiceModel;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework;

using Wave.Searchability.Data;

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
        ///     Compiles the filter that is used to query the feature layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="item">The item.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Return <see cref="IQueryFilter" /> representing the filter.
        /// </returns>
        protected override IQueryFilter CreateFilter(IFeatureLayer layer, string expression, SearchableLayer item, MapSearchServiceRequest request)
        {
            if (request.Extent == MapSearchServiceExtent.WithinAnyExtent)
                return base.CreateFilter(layer, expression, item, request);

            ISpatialFilter filter = new SpatialFilterClass();
            filter.WhereClause = expression;
            filter.Geometry = Document.ActiveView.Extent;
            filter.GeometryField = layer.FeatureClass.ShapeFieldName;
            filter.SpatialRel = (request.Extent == MapSearchServiceExtent.WithinOrOverlappingCurrentExtent)
                ? esriSpatialRelEnum.esriSpatialRelIntersects
                : esriSpatialRelEnum.esriSpatialRelContains;

            if (item.LayerDefinition)
            {
                IFeatureLayerDefinition featureLayerDefinition = (IFeatureLayerDefinition) layer;
                if (!string.IsNullOrEmpty(featureLayerDefinition.DefinitionExpression))
                    filter.WhereClause = string.Format("({0}) {1} ({2})", expression, LogicalOperator.And, featureLayerDefinition.DefinitionExpression);
            }

            return filter;
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