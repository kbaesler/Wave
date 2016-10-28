using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Allows you to create route features at a feature class level.
    /// </summary>
    /// <remarks>A route is any linear feature that has a unique identifier and measurement system stored with it</remarks>
    public sealed class Route
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Route" /> class.
        /// </summary>
        /// <param name="routeFeatureClass">The route feature class.</param>
        /// <param name="eventProperties">The event properties.</param>
        public Route(IFeatureClass routeFeatureClass, IRouteEventProperties eventProperties)
        {
            this.FeatureClass = routeFeatureClass;
            this.EventProperties = eventProperties;
            this.Errors = new List<string>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Route" /> class.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="workspace">The workspace.</param>
        public Route(RouteEventData<RouteMeasureLineSegmentation> eventData, IWorkspace workspace)
            : this(eventData.OutputTableName, workspace.GetFeatureClass("", eventData.RouteFeatureClassName), eventData.Segmentation, workspace, new QueryFilterClass() { WhereClause = eventData.WhereClause }, null)
        {
        }

        /// <summary>
        ///     Creates a route feature class used when measure values already exist as attributes of the input linear features.
        ///     That is, two attributes exist that represent from- and to-measure information for the input lines.
        /// </summary>
        /// <param name="outputRouteName">The name of the output route feature class.</param>
        /// <param name="source">The source (linear) feature class.</param>
        /// <param name="segmentation">The linear dynamic segmentation properties.</param>
        /// <param name="workspace">The location of the output route feature class.</param>
        /// <param name="filter">The filter used to query the source feature class for route data.</param>
        /// <param name="trackCancel">The object used to monitor the progress of the actions.</param>
        /// <remarks>
        ///     When using this method, it is important to orient each input linear feature in the direction of increasing
        ///     measure to prevent routes that have measures that do not always increase. In the following example, measure values
        ///     are obtained by using the values in the FR_M and TO_M fields. The digitized direction of the input features
        ///     determines the direction of the output route.
        /// </remarks>
        public Route(string outputRouteName, IFeatureClass source, RouteMeasureLineSegmentation segmentation, IWorkspace workspace, IQueryFilter filter, ITrackCancel trackCancel)
        {
            IRouteMeasureCreator2 gp = new RouteMeasureCreatorClass();
            gp.InputRouteIDFieldName = segmentation.EventRouteIDFieldName;

            if (source.HasOID && (filter != null && !string.IsNullOrEmpty(filter.WhereClause)))
            {
                IScratchWorkspaceFactory2 factory = new ScratchWorkspaceFactoryClass();
                var selectionContainer = factory.DefaultScratchWorkspace;

                var selection = source.Select(filter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, selectionContainer);
                gp.InputFeatureSelection = selection;
            }
            else
            {
                gp.InputFeatureClass = source;
            }

            var featureClassName = workspace.CreateDefinition(outputRouteName, new FeatureClassNameClass());
            workspace.Delete(featureClassName);

            var geometryDef = ((IClone)source.GetGeometryDef()).Clone() as IGeometryDef;
            var errors = gp.CreateUsing2Fields(segmentation.FromMeasureFieldName, segmentation.ToMeasureFieldName, featureClassName, geometryDef, "", trackCancel);

            this.Errors = new List<string>(errors.AsEnumerable());
            this.FeatureClass = workspace.GetFeatureClass("", outputRouteName);
            this.EventProperties = segmentation.EventProperties;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the errors that may have occured when creating the route.
        /// </summary>
        public List<string> Errors { get; private set; }

        /// <summary>
        ///     Gets the linear segmentation properties.
        /// </summary>
        public IRouteEventProperties EventProperties { get; private set; }

        /// <summary>
        ///     Gets the route feature class that was created.
        /// </summary>
        public IFeatureClass FeatureClass { get; private set; }

        /// <summary>
        ///     Gets the route locator.
        /// </summary>
        public IRouteLocator2 Locator
        {
            get { return ((IName)this.Name).Open() as IRouteLocator2; }
        }

        /// <summary>
        ///     Gets the name of the locator.
        /// </summary>
        public IRouteLocatorName Name
        {
            get
            {
                IRouteLocatorName locatorName = new RouteMeasureLocatorNameClass();
                locatorName.RouteFeatureClassName = ((IDataset)this.FeatureClass).FullName;
                locatorName.RouteMeasureUnit = this.EventProperties.EventMeasureUnit;
                locatorName.RouteIDFieldName = this.EventProperties.EventRouteIDFieldName;
                locatorName.RouteIDIsUnique = false;

                return locatorName;
            }
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Locate point features along the route and  writes the result to a new point event table.
        /// </summary>
        /// <param name="locatePointsAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="points">The points to locate.</param>
        /// <param name="searchRadius">If the input features are points, the search radius is a numeric value defining how far around each point a search
        /// will be done to find a target route. If the input features are lines, the search tolerance is really a cluster
        /// tolerance, which is a numeric value
        /// representing the maximum tolerated distance between the input lines and the target routes. If the input features
        /// are polygons, this parameter is ignored since no search radius is used.</param>
        /// <param name="searchMultipleLocations">If the point falls on more than one route for the given search radius, you can
        /// have the option to create multiple event records that correspond to each route in the search radius vicinity</param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">Allows you to include or disinclude the attributes of the point feature class.  If this is
        /// set to False, the output event table will only contain the route event properties.</param>
        /// <returns>
        /// Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePointsAlongRouteName, IFeatureClass points, double searchRadius, bool searchMultipleLocations, IQueryFilter filter, bool keepAllFields)
        {
            var properties = new RouteMeasurePointPropertiesClass();
            properties.EventRouteIDFieldName = "RID";
            properties.MeasureFieldName = "MEAS";

            return this.Locate(locatePointsAlongRouteName, points, searchRadius, searchMultipleLocations, properties, filter, keepAllFields);
        }

        /// <summary>
        /// Locate point features along the route and  writes the result to a new point event table.
        /// </summary>
        /// <param name="locatePointsAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="points">The points to locate.</param>
        /// <param name="searchRadius">If the input features are points, the search radius is a numeric value defining how far around each point a search
        /// will be done to find a target route. If the input features are lines, the search tolerance is really a cluster
        /// tolerance, which is a numeric value
        /// representing the maximum tolerated distance between the input lines and the target routes. If the input features
        /// are polygons, this parameter is ignored since no search radius is used.</param>
        /// <param name="searchMultipleLocations">If the point falls on more than one route for the given search radius, you can
        /// have the option to create multiple event records that correspond to each route in the search radius vicinity</param>
        /// <param name="properties">Parameter consisting of the route location fields and the type of events that will be written
        /// to the output event table.</param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">Allows you to include or disinclude the attributes of the point feature class.  If this is
        /// set to False, the output event table will only contain the route event properties.</param>
        /// <returns>
        /// Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePointsAlongRouteName, IFeatureClass points, double searchRadius, bool searchMultipleLocations, IRouteMeasurePointProperties properties, IQueryFilter filter, bool keepAllFields)
        {
            var locator = this.GetRouteLocatorOperations(filter, points);

            var ds = (IDataset)this.FeatureClass;
            var workspace = ds.Workspace;
            var outputClassName = workspace.CreateDefinition(locatePointsAlongRouteName, new TableNameClass());
            workspace.Delete(outputClassName);

            return locator.LocatePointFeatures(searchRadius, searchMultipleLocations, (IRouteEventProperties)properties, keepAllFields, outputClassName, "", null);
        }

        /// <summary>
        /// Locate line features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locateLinesAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="lines">The lines to locate.</param>
        /// <param name="clusterTolerance">The cluster tolerance which is a numeric value
        /// representing the maximum tolerated distance between the input lines and the target routes.</param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">Allows you to include or disinclude the attributes of the point feature class.  If this is
        /// set to False, the output event table will only contain the route event properties.</param>
        /// <returns>
        /// Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locateLinesAlongRouteName, IFeatureClass lines, double clusterTolerance, IQueryFilter filter, bool keepAllFields)
        {
            var properties = new RouteMeasureLinePropertiesClass();
            properties.EventRouteIDFieldName = "RID";
            properties.ToMeasureFieldName = "TMEAS";
            properties.FromMeasureFieldName = "FMEAS";

            return this.Locate(locateLinesAlongRouteName, lines, clusterTolerance, properties, filter, keepAllFields);
        }

        /// <summary>
        /// Locate line features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locateLinesAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="lines">The lines to locate.</param>
        /// <param name="clusterTolerance">The cluster tolerance which is a numeric value
        /// representing the maximum tolerated distance between the input lines and the target routes.</param>
        /// <param name="properties">Parameter consisting of the route location fields and the type of events that will be written
        /// to the output event table.</param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">Allows you to include or disinclude the attributes of the point feature class.  If this is
        /// set to False, the output event table will only contain the route event properties.</param>
        /// <returns>
        /// Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locateLinesAlongRouteName, IFeatureClass lines, double clusterTolerance, IRouteMeasureLineProperties properties, IQueryFilter filter, bool keepAllFields)
        {
            var locator = this.GetRouteLocatorOperations(filter, lines);

            var ds = (IDataset)this.FeatureClass;
            var workspace = ds.Workspace;
            var outputClassName = workspace.CreateDefinition(locateLinesAlongRouteName, new TableNameClass());
            workspace.Delete(outputClassName);

            return locator.LocateLineFeatures(clusterTolerance, (IRouteEventProperties)properties, keepAllFields, outputClassName, "", null);
        }

        /// <summary>
        /// Locates the polygon features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locatePolygonsAlongRouteName">Name of the locate polygons along route.</param>
        /// <param name="polygons">The polygons to locate.</param>
        /// <param name="keepZeroLengthEvents">if set to <c>true</c> allows you to keep or not keep the zero length line events in
        /// the output event table.  The zero length line events result from a case where the geometric intersection of the
        /// route (line) and the polygon is a point.</param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">Allows you to include or disinclude the attributes of the point feature class.  If this is
        /// set to False, the output event table will only contain the route event properties.</param>
        /// <returns>
        /// Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePolygonsAlongRouteName, IFeatureClass polygons, bool keepZeroLengthEvents, IQueryFilter filter, bool keepAllFields)
        {
            var properties = new RouteMeasureLinePropertiesClass();
            properties.EventRouteIDFieldName = "RID";
            properties.ToMeasureFieldName = "TMEAS";
            properties.FromMeasureFieldName = "FMEAS";

            return this.Locate(locatePolygonsAlongRouteName, polygons, keepZeroLengthEvents, properties, filter, keepAllFields);
        }

        /// <summary>
        /// Locates the polygon features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locatePolygonsAlongRouteName">Name of the locate polygons along route.</param>
        /// <param name="polygons">The polygons to locate.</param>
        /// <param name="keepZeroLengthEvents">if set to <c>true</c> allows you to keep or not keep the zero length line events in
        /// the output event table.  The zero length line events result from a case where the geometric intersection of the
        /// route (line) and the polygon is a point.</param>
        /// <param name="properties">Parameter consisting of the route location fields and the type of events that will be written
        /// to the output event table.</param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">Allows you to include or disinclude the attributes of the point feature class.  If this is
        /// set to False, the output event table will only contain the route event properties.</param>
        /// <returns>
        /// Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePolygonsAlongRouteName, IFeatureClass polygons, bool keepZeroLengthEvents, IRouteMeasureLineProperties properties, IQueryFilter filter, bool keepAllFields)
        {
            var locator = this.GetRouteLocatorOperations(filter, polygons);

            var ds = (IDataset)this.FeatureClass;
            var workspace = ds.Workspace;
            var outputClassName = workspace.CreateDefinition(locatePolygonsAlongRouteName, new TableNameClass());
            workspace.Delete(outputClassName);

            return locator.LocatePolygonFeatures((IRouteEventProperties)properties, keepAllFields, keepZeroLengthEvents, outputClassName, "", null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the route locator for the route.
        /// </summary>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        /// Returns a <see cred="IRouteLocatorOperations2" /> represents the route locator.
        /// </returns>
        private IRouteLocatorOperations2 GetRouteLocatorOperations(IQueryFilter filter, IFeatureClass source)
        {
            IRouteLocatorOperations2 locator = new RouteLocatorOperationsClass();
            locator.RouteIDFieldName = this.EventProperties.EventRouteIDFieldName;
            locator.RouteLocator = this.Locator;
            locator.InputFeatureClass = source;

            if (this.FeatureClass.HasOID && (filter != null && !string.IsNullOrEmpty(filter.WhereClause)))
            {
                IScratchWorkspaceFactory2 factory = new ScratchWorkspaceFactoryClass();
                var selectionContainer = factory.DefaultScratchWorkspace;

                var selection = this.FeatureClass.Select(filter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, selectionContainer);
                locator.RouteFeatureSelection = selection;
            }
            else
            {
                locator.RouteFeatureClass = this.FeatureClass;
            }

            return locator;
        }

        #endregion
    }

    /// <summary>
    ///     Serves an event table as a dynamic feature class. Every row in the table is served as a feature whose shape is
    ///     calculated on the fly every time it is requested. This is dynamic segmentation.
    /// </summary>
    public sealed class RouteEventSourceProxy
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteEventSourceProxy" /> class.
        /// </summary>
        /// <param name="eventTable">The event table that contains the event data.</param>
        /// <param name="routeFeatureClass">The route feature class.</param>
        /// <param name="eventProperties">The event properties.</param>
        /// <param name="routeIdIsUnique">if set to <c>true</c> when the route identifier is unique.</param>
        public RouteEventSourceProxy(ITable eventTable, IFeatureClass routeFeatureClass, IRouteEventProperties eventProperties, bool routeIdIsUnique)
            : this(eventTable, new RouteMeasureLocatorNameClass
            {
                RouteFeatureClassName = ((IDataset)routeFeatureClass).FullName,
                RouteMeasureUnit = eventProperties.EventMeasureUnit,
                RouteIDFieldName = eventProperties.EventRouteIDFieldName,
                RouteIDIsUnique = routeIdIsUnique
            }, eventProperties)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteEventSourceProxy" /> class.
        /// </summary>
        /// <param name="eventTable">The event table.</param>
        /// <param name="locatorName">Name of the locator.</param>
        /// <param name="eventProperties">The event properties.</param>
        public RouteEventSourceProxy(ITable eventTable, IRouteLocatorName locatorName, IRouteEventProperties eventProperties)
        {
            IRouteEventSourceName eventSourceName = new RouteEventSourceNameClass();
            eventSourceName.EventProperties = eventProperties;
            eventSourceName.EventTableName = ((IDataset)eventTable).FullName;
            eventSourceName.RouteLocatorName = locatorName;

            this.Name = eventSourceName;
            this.FeatureClass = (IRouteEventSource)((IName)eventSourceName).Open();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteEventSourceProxy" /> class.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="workspace">The workspace.</param>
        public RouteEventSourceProxy(RouteEventData<RouteMeasureLineSegmentation> eventData, IWorkspace workspace)
            : this(workspace.GetTable("", eventData.EventTableName), workspace.GetFeatureClass("", eventData.RouteFeatureClassName), eventData.Segmentation.EventProperties, eventData.RouteIdIsUnique)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the errors that occurred during dynamic segmentation.
        /// </summary>
        public IEnumerable<KeyValuePair<IRow, esriLocatingError>> Errors
        {
            get
            {
                IRow row;
                esriLocatingError error;
                IEventSourceErrors eventSourceErrors = (IEventSourceErrors)this.FeatureClass;
                IEnumEventError errors = eventSourceErrors.GetErrors();

                errors.Next(out row, out error);

                while (row != null)
                {
                    yield return new KeyValuePair<IRow, esriLocatingError>(row, error);

                    errors.Next(out row, out error);
                }
            }
        }

        /// <summary>
        ///     The event feature class.
        /// </summary>
        public IRouteEventSource FeatureClass { get; private set; }

        /// <summary>
        ///     The route event source name.
        /// </summary>
        public IRouteEventSourceName Name { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Converts the event route source to a stand-alone feature class.
        /// </summary>
        /// <param name="outputTableName">The name of the output feature class.</param>
        /// <param name="filter">The filter used to limit the number of features converted.</param>
        /// <param name="workspace">The workspace that will contain the feature class.</param>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the results of the conversion.
        /// </returns>
        public IFeatureClass Convert(string outputTableName, IQueryFilter filter, IWorkspace workspace)
        {
            var source = (IFeatureClass)this.FeatureClass;
            var geometryDef = ((IClone)source.GetGeometryDef()) as IGeometryDef;

            IFieldChecker checker = new FieldCheckerClass();
            IEnumFieldError errors;
            IFields fields;

            checker.InputWorkspace = workspace;
            checker.ValidateWorkspace = workspace;
            checker.Validate(source.Fields, out errors, out fields);

            var outputClassName = workspace.CreateDefinition(outputTableName, new FeatureClassNameClass());
            workspace.Delete(outputClassName);

            var dataConverter = new FeatureDataConverterClass();
            var invalidObjects = dataConverter.ConvertFeatureClass((IDatasetName)this.Name, filter, null, null, outputClassName, geometryDef, fields, "", 1000, 0);
            if (invalidObjects.AsEnumerable().Any()) return null;

            var route = workspace.GetFeatureClass("", outputTableName);
            return route;
        }

        #endregion
    }
}