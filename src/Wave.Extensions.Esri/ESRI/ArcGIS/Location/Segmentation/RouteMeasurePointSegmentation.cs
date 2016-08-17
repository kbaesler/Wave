using System.Runtime.Serialization;
using System.Xml.Serialization;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     The data requirements for a performing dynamic segmenation on point features or point event tables or point route
    ///     locations
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasurePointProperties" />
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasureSegmentation" />
    public interface IRouteMeasurePointSegmentation : IRouteMeasurePointProperties, IRouteMeasureSegmentation
    {
    }

    /// <summary>
    ///     A configuration use for dynamic segmentation for point event tables.
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasurePointSegmentation" />
    [DataContract]
    public class RouteMeasurePointSegmentation : IRouteMeasurePointSegmentation
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteMeasurePointSegmentation" /> class.
        /// </summary>
        public RouteMeasurePointSegmentation()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteMeasurePointSegmentation" /> class.
        /// </summary>
        /// <param name="measureFieldName">The name of the measure field.</param>
        /// <param name="routeIDFieldName">The name of the route field.</param>
        public RouteMeasurePointSegmentation(string measureFieldName, string routeIDFieldName)
        {
            this.MeasureFieldName = measureFieldName;
            this.RouteIDFieldName = routeIDFieldName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the route measure units.
        /// </summary>
        [DataMember]
        public esriUnits RouteMeasureUnit { get; set; }

        #endregion

        #region IRouteMeasurePointSegmentation Members

        /// <summary>
        ///     Gets or sets the name of the measure field that defines the event event’s location on the route
        /// </summary>
        /// <value>The name of the measure field.</value>
        [DataMember]
        public string MeasureFieldName { get; set; }

        /// <summary>
        ///     Gets or sets the route event properties.
        /// </summary>
        /// <value>The route event properties.</value>
        [XmlIgnore]
        public IRouteEventProperties2 RouteEventProperties
        {
            get
            {
                IRouteMeasurePointProperties point = new RouteMeasurePointPropertiesClass();
                point.MeasureFieldName = this.MeasureFieldName;

                IRouteEventProperties2 props = (IRouteEventProperties2) point;
                props.EventMeasureUnit = this.RouteMeasureUnit;
                props.EventRouteIDFieldName = this.RouteIDFieldName;

                return props;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the route ID field that identifies route on which event is located.
        /// </summary>
        /// <value>The name of the route ID field.</value>
        [DataMember]
        public string RouteIDFieldName { get; set; }

        #endregion
    }
}