using System.Runtime.Serialization;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Data requirements for performing dynamic segmentation.
    ///     <remarks>
    ///         <para>
    ///             Each feature (commonly called a route) must have a unique identifier and a measurement system stored with
    ///             it. The measurement system
    ///             will vary depending on a linear or point feature.
    ///         </para>
    ///         <para>
    ///             1. Each event in an event table must include a unique identifier its measurement along a linear feature.
    ///             2. Each feature must have a unique identifier and a measurement system stored with it.
    ///         </para>
    ///     </remarks>
    /// </summary>
    public interface IRouteMeasureSegmentation
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets units of the event measure(s).
        /// </summary>
        /// <value>
        ///     The event measure unit.
        /// </value>
        [DataMember]
        esriUnits EventMeasureUnit { get; set; }

        /// <summary>
        ///     Gets the route event properties.
        /// </summary>
        /// <returns>Returns a <see cref="IRouteEventProperties" /> for the segmentation</returns>
        IRouteEventProperties2 EventProperties { get; }

        /// <summary>
        ///     Gets or sets the name of the event route identifier field.
        /// </summary>
        /// <value>
        ///     The name of the event route identifier field.
        /// </value>
        [DataMember]
        string EventRouteIDFieldName { get; set; }

        #endregion
    }

    /// <summary>
    ///     Data requirements for performing dynamic segmentation.
    ///     <remarks>
    ///         <para>
    ///             Each feature (commonly called a route) must have a unique identifier and a measurement system stored with
    ///             it. The measurement system
    ///             will vary depending on a linear or point feature.
    ///         </para>
    ///         <para>
    ///             1. Each event in an event table must include a unique identifier its measurement along a linear feature.
    ///             2. Each feature must have a unique identifier and a measurement system stored with it.
    ///         </para>
    ///     </remarks>
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.IRouteMeasureSegmentation" />
    [DataContract]
    public abstract class RouteMeasureSegmentation : IRouteMeasureSegmentation
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteMeasureSegmentation" /> class.
        /// </summary>
        protected RouteMeasureSegmentation()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteMeasureSegmentation" /> class.
        /// </summary>
        /// <param name="routeIDFieldName">The name of the route field.</param>
        /// <param name="measureUnits">The measure units.</param>
        protected RouteMeasureSegmentation(string routeIDFieldName, esriUnits measureUnits)
        {
            this.EventRouteIDFieldName = routeIDFieldName;
            this.EventMeasureUnit = measureUnits;
        }

        #endregion

        #region IRouteMeasureSegmentation Members

        /// <summary>
        ///     Gets the route event properties.
        /// </summary>
        /// <returns>
        ///     Returns a <see cref="IRouteEventProperties" /> for the segmentation
        /// </returns>
        public abstract IRouteEventProperties2 EventProperties { get; }

        /// <summary>
        ///     Gets or sets units of the event measure(s).
        /// </summary>
        /// <value>
        ///     The event measure unit.
        /// </value>
        [DataMember]
        public esriUnits EventMeasureUnit { get; set; }

        /// <summary>
        ///     Gets or sets the name of the event route identifier field.
        /// </summary>
        /// <value>
        ///     The name of the event route identifier field.
        /// </value>
        [DataMember]
        public string EventRouteIDFieldName { get; set; }

        #endregion
    }
}