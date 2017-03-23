using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ESRI.ArcGIS.Location.EventData{T}" />
    [DataContract]
    [KnownType(typeof(RouteMeasureLineSegmentation))]
    [KnownType(typeof(RouteMeasurePointSegmentation))]
    public class RouteEventData<T> : EventData<T> where T : RouteMeasureSegmentation
    {
        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="RouteEventData{T}" />
        /// </summary>
        public RouteEventData()
        {
        }

        /// <summary>
        ///     Initializes the <see cref="RouteEventData{T}" />
        /// </summary>
        /// <param name="routeFeatureClassName">The name of the input feature class for the route data.</param>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        public RouteEventData(string routeFeatureClassName, string eventTableName, string outputTableName, T segmentation)
            : this(routeFeatureClassName, eventTableName, outputTableName, "", segmentation)
        {
        }

        /// <summary>
        ///     Initializes the <see cref="RouteEventData{T}" />
        /// </summary>
        /// <param name="routeFeatureClassName">The name of the input feature class for the route data.</param>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        public RouteEventData(string routeFeatureClassName, string eventTableName, string outputTableName, string whereClause, T segmentation)
            : base(eventTableName, outputTableName, whereClause, segmentation)
        {
            this.RouteFeatureClassName = routeFeatureClassName;
        }

        /// <summary>
        ///     Initializes the <see cref="RouteEventData{T}" />
        /// </summary>
        /// <param name="routeFeatureClassName">The name of the input feature class for the route data.</param>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        /// <param name="routeIdIsUnique">if set to <c>true</c> when the route identifier is unique.</param>
        public RouteEventData(string routeFeatureClassName, string eventTableName, string outputTableName, string whereClause, T segmentation, bool routeIdIsUnique)
            : base(eventTableName, outputTableName, whereClause, segmentation)
        {
            this.RouteFeatureClassName = routeFeatureClassName;
            this.RouteIdIsUnique = routeIdIsUnique;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the input route feature class name.
        /// </summary>
        [DataMember]
        public string RouteFeatureClassName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the route identifier is unique.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the route identifier is unique; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool RouteIdIsUnique { get; set; }

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
        public EventData(string eventTableName)
            : this(eventTableName, eventTableName.Replace('.', '_'), "")
        {
        }

        /// <summary>
        ///     Initializes the <see cref="EventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        public EventData(string eventTableName, string whereClause)
            : this(eventTableName)
        {
            this.WhereClause = whereClause;
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

    /// <summary>
    /// </summary>
    [DataContract]
    [KnownType(typeof(RouteMeasureLineSegmentation))]
    [KnownType(typeof(RouteMeasurePointSegmentation))]
    public class EventData<T> : EventData where T : RouteMeasureSegmentation
    {
        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="EventData{T}" />
        /// </summary>
        public EventData()
        {
        }

        /// <summary>
        ///     Initializes the <see cref="EventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        public EventData(string eventTableName, string outputTableName, T segmentation)
            : this(eventTableName, outputTableName, "", segmentation)
        {
        }

        /// <summary>
        ///     Initializes the <see cref="EventData" />
        /// </summary>
        /// <param name="eventTableName">The name of the event table.</param>
        /// <param name="outputTableName">The name of the output table.</param>
        /// <param name="whereClause">A filter that is applied to the event table.</param>
        /// <param name="segmentation">The event table segmentation properties</param>
        public EventData(string eventTableName, string outputTableName, string whereClause, T segmentation)
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
}