using System.Configuration;

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
        string MeasureFieldName { get; set; }

        #endregion
    }

    /// <summary>
    ///     A configuration use for dynamic segmentation for point event tables.
    /// </summary>
    /// <example>
    ///     <segmentation routeIDFieldName="PIPELINE_ID" measureFieldName="BEGCUMSTA" />
    /// </example>
    public class PointSegmentation : ConfigurationElement, IPointSegmentation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the route measure units.
        /// </summary>
        [ConfigurationProperty("routeMeasureUnit", DefaultValue = esriUnits.esriFeet)]
        public esriUnits RouteMeasureUnit
        {
            get { return (esriUnits) this["routeMeasureUnit"]; }
            set { this["routeMeasureUnit"] = value; }
        }

        #endregion

        #region IPointSegmentation Members

        /// <summary>
        ///     Gets or sets the name of the measure field that defines the event event’s location on the route
        /// </summary>
        /// <value>The name of the measure field.</value>
        [ConfigurationProperty("measureFieldName", IsRequired = true)]
        public string MeasureFieldName
        {
            get { return (string) this["measureFieldName"]; }
            set { this["measureFieldName"] = value; }
        }

        /// <summary>
        ///     Gets or sets the route event properties.
        /// </summary>
        /// <value>The route event properties.</value>
        public IRouteEventProperties2 RouteEventProperties
        {
            get
            {
                IRouteMeasurePointProperties props = new RouteMeasurePointPropertiesClass();
                props.MeasureFieldName = this.MeasureFieldName;

                IRouteEventProperties2 eventProps = (IRouteEventProperties2) props;
                eventProps.EventMeasureUnit = this.RouteMeasureUnit;
                eventProps.EventRouteIDFieldName = this.RouteIDFieldName;

                return eventProps;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the route ID field that identifies route on which event is located.
        /// </summary>
        /// <value>The name of the route ID field.</value>
        [ConfigurationProperty("routeIDFieldName", IsRequired = true)]
        public string RouteIDFieldName
        {
            get { return (string) this["routeIDFieldName"]; }
            set { this["routeIDFieldName"] = value; }
        }

        #endregion
    }
}