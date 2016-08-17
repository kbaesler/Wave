using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.EventData" />
    [DataContract]
    [KnownType(typeof (RouteMeasureLineSegmentation))]
    public class LayerRouteEventData : EventData
    {
        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="LayerRouteEventData" />
        /// </summary>
        public LayerRouteEventData()
        {
        }

        /// <summary>
        ///     Initializes the <see cref="LayerRouteEventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        /// <param name="routeFeatureClassName">The name of the route table.</param>
        /// <param name="segmentation">The linear segmentation properties</param>
        public LayerRouteEventData(string eventTableName, string outputTableName, string whereClause, string routeFeatureClassName, IRouteMeasureLineSegmentation segmentation)
            : base(eventTableName, outputTableName, whereClause)
        {
            this.Route = routeFeatureClassName;
            this.Segmentation = segmentation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the route.
        /// </summary>
        /// <value>
        ///     The route.
        /// </value>
        [DataMember]
        public string Route { get; set; }

        /// <summary>
        ///     Gets or sets the segmentation.
        /// </summary>
        /// <value>
        ///     The segmentation.
        /// </value>
        [DataMember]
        public IRouteMeasureLineSegmentation Segmentation { get; set; }

        #endregion
    }

    /// <summary>
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteEventData{IRouteMeasureSegmentation}" />
    [DataContract]
    [KnownType(typeof (RouteMeasureLineSegmentation))]
    [KnownType(typeof (RouteMeasurePointSegmentation))]
    public class RouteEventData : RouteEventData<IRouteMeasureSegmentation>
    {
        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="RouteEventData" />
        /// </summary>
        public RouteEventData()
        {
        }

        /// <summary>
        ///     Initializes the <see cref="RouteEventData" />
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        public RouteEventData(EventData eventData, IRouteMeasureSegmentation segmentation)
            : base(eventData.EventTableName, eventData.OutputTableName, eventData.WhereClause, segmentation)
        {
        }

        /// <summary>
        ///     Initializes the <see cref="RouteEventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        public RouteEventData(string eventTableName, string outputTableName, string whereClause, IRouteMeasureSegmentation segmentation)
            : base(eventTableName, outputTableName, whereClause, segmentation)
        {
        }

        #endregion
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteEventData{IRouteMeasureSegmentation}" />
    [DataContract]
    [KnownType(typeof (RouteMeasureLineSegmentation))]
    [KnownType(typeof (RouteMeasurePointSegmentation))]
    public class RouteEventData<T> : EventData
    {
        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="RouteEventData" />
        /// </summary>
        public RouteEventData()
        {
        }

        /// <summary>
        ///     Initializes the <see cref="RouteEventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        public RouteEventData(string eventTableName, string outputTableName, string whereClause, T segmentation)
            : base(eventTableName, outputTableName, whereClause)
        {
            this.Segmentation = segmentation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the event table segmentation properties.
        /// </summary>
        [DataMember]
        public T Segmentation { get; set; }

        #endregion
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public class EventData
    {
        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="EventData" />
        /// </summary>
        public EventData()
        {
        }

        /// <summary>
        ///     Initializes the <see cref="EventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        public EventData(string eventTableName, string outputTableName)
            : this(eventTableName, outputTableName, "")
        {
        }

        /// <summary>
        ///     Initializes the <see cref="EventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        public EventData(string eventTableName, string outputTableName, string whereClause)
        {
            this.EventTableName = eventTableName;
            this.OutputTableName = outputTableName;
            this.WhereClause = whereClause;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the event table.
        /// </summary>
        /// <value>The name of the event table.</value>
        [DataMember]
        public string EventTableName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the output table.
        /// </summary>
        /// <value>The name of the output table.</value>
        [DataMember]
        public string OutputTableName { get; set; }

        /// <summary>
        ///     Gets the where clause to filter the event data.
        /// </summary>
        [DataMember]
        public string WhereClause { get; set; }

        #endregion
    }
}