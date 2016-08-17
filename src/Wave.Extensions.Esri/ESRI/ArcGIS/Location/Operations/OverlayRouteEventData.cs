using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteEventData" />
    [DataContract]
    public class OverlayRouteEventData : RouteEventData
    {
        #region Constructors

        public OverlayRouteEventData(string eventTableName, string outputTableName, IRouteMeasureSegmentation segmentation, string overlayTableName, OverlayType overlayType)
            : this(eventTableName, outputTableName, "", segmentation, overlayTableName, segmentation, overlayType)
        {
        }

        public OverlayRouteEventData(string eventTableName, string outputTableName, IRouteMeasureSegmentation segmentation, string overlayTableName, IRouteMeasureSegmentation overlaySegmentation, OverlayType overlayType)
            : this(eventTableName, outputTableName, "", segmentation, overlayTableName, overlaySegmentation, overlayType)
        {
        }

        public OverlayRouteEventData(string eventTableName, string outputTableName, string whereClause, IRouteMeasureSegmentation segmentation, string overlayTableName, IRouteMeasureSegmentation overlaySegmentation, OverlayType overlayType)
            : base(eventTableName, outputTableName, whereClause, segmentation)
        {
            this.OverlaySegmentation = overlaySegmentation;
            this.OverlayTableName = overlayTableName;
            this.OverlayType = overlayType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the overlay table segmentation properties.
        /// </summary>
        [DataMember]
        public IRouteMeasureSegmentation OverlaySegmentation { get; set; }

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