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
    public interface IDynamicSegmentation
    {
        #region Public Properties

        /// <summary>
        ///     Gets the route event properties.
        /// </summary>
        /// <value>The route event properties.</value>
        IRouteEventProperties2 RouteEventProperties { get; }

        /// <summary>
        ///     Gets or sets the name of the route ID field that identifies route on which event is located.
        /// </summary>
        /// <value>The name of the route ID field.</value>
        string RouteIDFieldName { get; set; }

        #endregion
    }    
}