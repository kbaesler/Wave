using System.Runtime.Serialization;
using System.Xml.Serialization;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// The data requirements for a performing dynamic segmenation on point features or point event tables or point route
    /// locations
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteMeasureSegmentation" />
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasurePointProperties" />
    [DataContract]
    public class RouteMeasurePointSegmentation : RouteMeasureSegmentation, IRouteMeasurePointProperties
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
            : base(routeIDFieldName, esriUnits.esriUnknownUnits)
        {
            this.MeasureFieldName = measureFieldName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the route event properties.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="IRouteEventProperties" /> for the segmentation
        /// </returns>
        /// <value>
        ///     The route event properties.
        /// </value>
        public override IRouteEventProperties2 EventProperties
        {
            get
            {
                IRouteMeasurePointProperties points = new RouteMeasurePointPropertiesClass();
                points.MeasureFieldName = this.MeasureFieldName;

                IRouteEventProperties2 props = (IRouteEventProperties2)points;
                props.EventMeasureUnit = this.EventMeasureUnit;
                props.EventRouteIDFieldName = this.EventRouteIDFieldName;

                return props;
            }
        }

        #endregion

        #region IRouteMeasurePointProperties Members

        /// <summary>
        ///     Gets or sets the name of the measure field that defines the event event’s location on the route
        /// </summary>
        /// <value>The name of the measure field.</value>
        [DataMember]
        public string MeasureFieldName { get; set; }

        #endregion
    }
}