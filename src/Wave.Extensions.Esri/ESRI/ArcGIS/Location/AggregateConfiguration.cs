using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class AggregateConfiguration
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggregateConfiguration" /> class.
        /// </summary>
        public AggregateConfiguration()
        {
            this.Exports = new List<EventData>();
            this.Overlays = new List<OverlayRouteEventData>();
            this.Dissolves = new List<DissolveRouteEventData>();
            this.Routes = new List<RouteEventData<RouteMeasureLineSegmentation>>();
            this.Concatenates = new List<ConcatenateRouteEventData>();
            this.Joins = new List<JoinEventData>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the route event data that will be concatenated to analyze the data.
        /// </summary>
        [DataMember]
        public List<ConcatenateRouteEventData> Concatenates { get; set; }

        /// <summary>
        ///     Gets the route event data that will be dissolved to analyze the data.
        /// </summary>
        [DataMember]
        public List<DissolveRouteEventData> Dissolves { get; set; }

        /// <summary>
        ///     Gets event tables that will exported.
        /// </summary>
        [DataMember]
        public List<EventData> Exports { get; set; }

        /// <summary>
        ///     Gets the joins.
        /// </summary>
        /// <value>
        ///     The joins.
        /// </value>
        [DataMember]
        public List<JoinEventData> Joins { get; set; }

        /// <summary>
        ///     Gets the route event data that will be overlayed to analyze the data.
        /// </summary>
        [DataMember]
        public List<OverlayRouteEventData> Overlays { get; set; }

        /// <summary>
        ///     Gets the routes that will be created.
        /// </summary>
        [DataMember]
        public List<RouteEventData<RouteMeasureLineSegmentation>> Routes { get; set; }

        #endregion
    }
}