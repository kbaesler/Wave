using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteEventData{IRouteMeasureSegmentation}" />
    [DataContract]
    public class OverlayRouteEventData : EventData<RouteMeasureSegmentation>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayRouteEventData" /> class.
        /// </summary>
        /// <param name="eventTableName">Name of the event table.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="segmentation">The segmentation for the event and overlay tables.</param>
        /// <param name="overlayTableName">Name of the overlay table.</param>
        /// <param name="overlayType">Type of the overlay.</param>
        public OverlayRouteEventData(string eventTableName, string outputTableName, RouteMeasureSegmentation segmentation, string overlayTableName, OverlayType overlayType)
            : this(eventTableName, outputTableName, "", segmentation, overlayTableName, segmentation, overlayType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayRouteEventData" /> class.
        /// </summary>
        /// <param name="eventTableName">Name of the event table.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="eventSegmentation">The event segmentation.</param>
        /// <param name="overlayTableName">Name of the overlay table.</param>
        /// <param name="overlaySegmentation">The overlay segmentation.</param>
        /// <param name="overlayType">Type of the overlay.</param>
        public OverlayRouteEventData(string eventTableName, string outputTableName, RouteMeasureSegmentation eventSegmentation, string overlayTableName, RouteMeasureSegmentation overlaySegmentation, OverlayType overlayType)
            : this(eventTableName, outputTableName, "", eventSegmentation, overlayTableName, overlaySegmentation, overlayType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayRouteEventData" /> class.
        /// </summary>
        /// <param name="eventTableName">Name of the event table.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="eventSegmentation">The event segmentation.</param>
        /// <param name="overlayTableName">Name of the overlay table.</param>
        /// <param name="overlaySegmentation">The overlay segmentation.</param>
        /// <param name="overlayType">Type of the overlay.</param>
        public OverlayRouteEventData(string eventTableName, string outputTableName, string whereClause, RouteMeasureSegmentation eventSegmentation, string overlayTableName, RouteMeasureSegmentation overlaySegmentation, OverlayType overlayType)
            : base(eventTableName, outputTableName, whereClause, eventSegmentation)
        {
            this.OverlaySegmentation = overlaySegmentation;
            this.OverlayTableName = overlayTableName;
            this.OverlayType = overlayType;
            this.KeepAllFields = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether to keep all fields in the resultant table.
        /// </summary>
        /// <value>
        ///     <c>true</c> if keeping all fields in the resultant table; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool KeepAllFields { get; set; }

        /// <summary>
        ///     Gets the overlay table segmentation properties.
        /// </summary>
        [DataMember]
        public RouteMeasureSegmentation OverlaySegmentation { get; set; }

        /// <summary>
        ///     Gets the name of the overlay table.
        /// </summary>
        [DataMember]
        public string OverlayTableName { get; set; }

        /// <summary>
        ///     Gets the type of the overlay.
        /// </summary>
        [DataMember]
        public OverlayType OverlayType { get; set; }

        #endregion
    }

    /// <summary>
    ///     The type of overlay operations.
    /// </summary>
    public enum OverlayType
    {
        /// <summary>
        ///     Writes only overlapping events to the output event table.
        /// </summary>
        Intersect,

        /// <summary>
        ///     Writes all events to the output table. Linear events are split at their intersections.
        /// </summary>
        Union
    }
}