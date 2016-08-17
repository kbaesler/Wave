using System.Runtime.Serialization;
using System.Xml.Serialization;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     The data requirements for a performing dynamic segmenation on linear features or line event tables or line route
    ///     locations
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasureLineProperties" />
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasureSegmentation" />
    public interface IRouteMeasureLineSegmentation : IRouteMeasureLineProperties, IRouteMeasureSegmentation
    {
    }

    /// <summary>
    ///     A configuration use for dynamic segmentation for linear event tables.
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasureLineSegmentation" />
    [DataContract]
    public class RouteMeasureLineSegmentation : IRouteMeasureLineSegmentation
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteMeasureLineSegmentation" /> class.
        /// </summary>
        public RouteMeasureLineSegmentation()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteMeasureLineSegmentation" /> class.
        /// </summary>
        /// <param name="fromMeasureFieldName">The name of the from measure field.</param>
        /// <param name="toMeasureFieldName">The name of the to measure field.</param>
        /// <param name="routeIDFieldName">The name of the route field.</param>
        public RouteMeasureLineSegmentation(string fromMeasureFieldName, string toMeasureFieldName, string routeIDFieldName)
        {
            this.FromMeasureFieldName = fromMeasureFieldName;
            this.ToMeasureFieldName = toMeasureFieldName;
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

        #region IRouteMeasureLineSegmentation Members

        /// <summary>
        ///     Gets or sets the name of from measure field that defins the end of the line event.
        /// </summary>
        /// <value>The name of from measure field.</value>
        [DataMember]
        public string FromMeasureFieldName { get; set; }

        /// <summary>
        ///     Gets or sets the route event properties.
        /// </summary>
        /// <value>The route event properties.</value>
        [XmlIgnore]
        public IRouteEventProperties2 RouteEventProperties
        {
            get
            {
                IRouteMeasureLineProperties line = new RouteMeasureLinePropertiesClass();
                line.FromMeasureFieldName = this.FromMeasureFieldName;
                line.ToMeasureFieldName = this.ToMeasureFieldName;

                IRouteEventProperties2 props = (IRouteEventProperties2) line;
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

        /// <summary>
        ///     Gets or sets the name of to measure field that defines the beginning of line event.
        /// </summary>
        /// <value>The name of to measure field.</value>
        [DataMember]
        public string ToMeasureFieldName { get; set; }

        #endregion
    }
}