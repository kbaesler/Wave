using System.Runtime.Serialization;
using System.Xml.Serialization;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// The data requirements for a performing dynamic segmenation on linear features or line event tables or line route
    /// locations
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteMeasureSegmentation" />
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasureLineProperties" />
    [DataContract]
    public class RouteMeasureLineSegmentation : RouteMeasureSegmentation, IRouteMeasureLineProperties
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
            : base(routeIDFieldName, esriUnits.esriUnknownUnits)
        {
            this.FromMeasureFieldName = fromMeasureFieldName;
            this.ToMeasureFieldName = toMeasureFieldName;
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
                IRouteMeasureLineProperties line = new RouteMeasureLinePropertiesClass();
                line.FromMeasureFieldName = this.FromMeasureFieldName;
                line.ToMeasureFieldName = this.ToMeasureFieldName;

                IRouteEventProperties2 props = (IRouteEventProperties2)line;
                props.EventMeasureUnit = this.EventMeasureUnit;
                props.EventRouteIDFieldName = this.EventRouteIDFieldName;

                return props;
            }
        }

        #endregion

        #region IRouteMeasureLineProperties Members

        /// <summary>
        ///     Gets or sets the name of from measure field that defins the end of the line event.
        /// </summary>
        /// <value>The name of from measure field.</value>
        [DataMember]
        public string FromMeasureFieldName { get; set; }


        /// <summary>
        ///     Gets or sets the name of to measure field that defines the beginning of line event.
        /// </summary>
        /// <value>The name of to measure field.</value>
        [DataMember]
        public string ToMeasureFieldName { get; set; }

        #endregion
    }
}