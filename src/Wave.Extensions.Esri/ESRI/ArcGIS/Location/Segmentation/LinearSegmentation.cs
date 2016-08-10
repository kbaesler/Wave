using System.Configuration;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     The data requirements for a performing dynamic segmenation on linear features or line event tables or line route
    ///     locations
    /// </summary>
    public interface ILinearSegmentation : IDynamicSegmentation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of from measure field that defines the end of the line event.
        /// </summary>
        /// <value>The name of from measure field.</value>
        string FromMeasureFieldName { get; set; }

        /// <summary>
        ///     Gets or sets the name of to measure field that defines the beginning of line event.
        /// </summary>
        /// <value>The name of to measure field.</value>
        string ToMeasureFieldName { get; set; }

        #endregion
    }

    /// <summary>
    ///     A configuration use for dynamic segmentation for linear event tables.
    /// </summary>
    /// <example>
    ///     <segmentation routeIDFieldName="PIPELINE_ID" toMeasureFieldName="ENDCUMSTA" fromMeasureFieldName="BEGCUMSTA" />
    /// </example>
    public class LinearSegmentation : ConfigurationElement, ILinearSegmentation
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

        #region ILinearSegmentation Members

        /// <summary>
        ///     Gets or sets the name of from measure field that defins the end of the line event.
        /// </summary>
        /// <value>The name of from measure field.</value>
        [ConfigurationProperty("fromMeasureFieldName", IsRequired = true)]
        public string FromMeasureFieldName
        {
            get { return (string) this["fromMeasureFieldName"]; }
            set { this["fromMeasureFieldName"] = value; }
        }


        /// <summary>
        ///     Gets or sets the route event properties.
        /// </summary>
        /// <value>The route event properties.</value>
        public IRouteEventProperties2 RouteEventProperties
        {
            get
            {
                IRouteMeasureLineProperties props = new RouteMeasureLinePropertiesClass();
                props.FromMeasureFieldName = this.FromMeasureFieldName;
                props.ToMeasureFieldName = this.ToMeasureFieldName;

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
            get { return (string) base["routeIDFieldName"]; }
            set { base["routeIDFieldName"] = value; }
        }

        /// <summary>
        ///     Gets or sets the name of to measure field that defines the beginning of line event.
        /// </summary>
        /// <value>The name of to measure field.</value>
        public string ToMeasureFieldName
        {
            get { return (string) this["toMeasureFieldName"]; }
            set { this["toMeasureFieldName"] = value; }
        }

        #endregion
    }
}