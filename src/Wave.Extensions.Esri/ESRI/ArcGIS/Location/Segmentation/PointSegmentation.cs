using System.Configuration;
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
    public interface IPointSegmentation : IDynamicSegmentation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the measure field that defines the event event’s location on the route
        /// </summary>
        /// <value>The name of the measure field.</value>
        [DataMember]
        string MeasureFieldName { get; set; }

        #endregion
    }

    /// <summary>
    ///     A configuration use for dynamic segmentation for point event tables.
    /// </summary>
    [DataContract]
    public class PointSegmentation : IPointSegmentation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the route measure units.
        /// </summary>
        [DataMember]
        public esriUnits RouteMeasureUnit { get; set; }

        #endregion

        #region IPointSegmentation Members

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
                IRouteMeasurePointProperties props = new RouteMeasurePointPropertiesClass();
                props.MeasureFieldName = this.MeasureFieldName;

                IRouteEventProperties2 eventProps = (IRouteEventProperties2)props;
                eventProps.EventMeasureUnit = this.RouteMeasureUnit;
                eventProps.EventRouteIDFieldName = this.RouteIDFieldName;

                return eventProps;
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