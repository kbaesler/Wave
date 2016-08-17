using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    [DataContract]
    public class SegmentationData
    {
        #region Constructors

        public SegmentationData()
        {
            this.Exports = new List<EventData>();
            this.Overlays = new List<OverlayRouteEventData>();
            this.Dissolves = new List<DissolveRouteEventData>();
            this.Routes = new List<LayerRouteEventData>();
            this.Concatenates = new List<ConcatenateRouteEventData>();
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
        ///     Gets the route event data that will be overlayed to analyze the data.
        /// </summary>
        [DataMember]
        public List<OverlayRouteEventData> Overlays { get; set; }

        /// <summary>
        ///     Gets the routes that will be created.
        /// </summary>
        [DataMember]
        public List<LayerRouteEventData> Routes { get; set; }

        #endregion
    }
}