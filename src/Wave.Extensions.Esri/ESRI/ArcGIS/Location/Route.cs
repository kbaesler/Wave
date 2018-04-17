using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Allows you to create route features at a feature class level.
    /// </summary>
    /// <remarks>
    ///     A route is any linear feature that has a unique identifier and measurement system stored with it
    /// </remarks>
    public sealed class Route
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Route" /> class.
        /// </summary>
        /// <param name="routeFeatureClass">A polyline feature class with m-values.</param>
        /// <param name="routeIDFieldName">Any numeric or text field containing route identifiers.</param>
        /// <param name="routeIDIsUnique">if set to <c>true</c> when the route identifier is unique.</param>
        public Route(IFeatureClass routeFeatureClass, string routeIDFieldName, bool routeIDIsUnique)
            : this(routeFeatureClass, routeIDFieldName, routeIDIsUnique, null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Route" /> class.
        /// </summary>
        /// <param name="routeFeatureClass">A polyline feature class with m-values.</param>
        /// <param name="routeIDFieldName">Any numeric or text field containing route identifiers.</param>
        /// <param name="routeIDIsUnique">if set to <c>true</c> when the route identifier is unique.</param>
        /// <param name="routeWhereClause">The string that limits the number of routes on which route locations can be found.</param>
        public Route(IFeatureClass routeFeatureClass, string routeIDFieldName, bool routeIDIsUnique, string routeWhereClause)
            : this(routeFeatureClass, routeIDFieldName, esriUnits.esriUnknownUnits, routeIDIsUnique, routeWhereClause)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Route" /> class.
        /// </summary>
        /// <param name="routeFeatureClass">A polyline feature class with m-values.</param>
        /// <param name="routeIDFieldName">Any numeric or text field containing route identifiers.</param>
        /// <param name="routeMeasureUnits">The units of the m-values stored in the routes. The default is esriUnknownUnits.</param>
        /// <param name="routeIDIsUnique">if set to <c>true</c> when the route identifier is unique.</param>
        /// <param name="routeWhereClause">The string that limits the number of routes on which route locations can be found.</param>
        public Route(IFeatureClass routeFeatureClass, string routeIDFieldName, esriUnits routeMeasureUnits, bool routeIDIsUnique, string routeWhereClause)
        {
            this.FeatureClass = routeFeatureClass;
            this.Name = this.GetRouteMeasureLocatorName(routeFeatureClass, routeIDFieldName, routeMeasureUnits, routeIDIsUnique, routeWhereClause);
            this.Locator = (IRouteLocator2)((IName)this.Name).Open();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the route locator.
        /// </summary>
        public IRouteLocator2 Locator
        {
            get; private set;
        }

        /// <summary>
        ///     Gets the route feature class that was created.
        /// </summary>
        public IFeatureClass FeatureClass { get; private set; }

        /// <summary>
        ///     Gets the name of the locator.
        /// </summary>
        public IRouteLocatorName Name { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Identify route locations in an envelope.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{T}" /> representing the route and location.
        /// </returns>
        public List<RouteLocationResult> Identify(IGeometry buffer, string whereClause, double tolerance)
        {
            var searchEnvelope = buffer.Envelope;
            searchEnvelope.Expand(tolerance, tolerance, false);

            return Identify(searchEnvelope, whereClause);
        }

        /// <summary>
        /// Identify route locations in an envelope.
        /// </summary>
        /// <param name="searchEnvelope">The search envelope.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <returns>
        /// Returns a <see cref="IEnumerable{T}"/> representing the route and location.
        /// </returns>
        public List<RouteLocationResult> Identify(IEnvelope searchEnvelope, string whereClause)
        {
            List<RouteLocationResult> list = new List<RouteLocationResult>();

            var values = this.Locator.Identify(searchEnvelope, whereClause);
            values.Reset();

            for (int i = 0; i < values.Count; i++)
            {
                IRouteLocation location;
                IFeature feature;
                values.Next(out location, out feature);

                IGeometry geometry;
                esriLocatingError error;
                this.Locator.Locate(location, out geometry, out error);

                list.Add(new RouteLocationResult(location, feature, geometry, error));
            }

            return list;
        }

        /// <summary>
        ///     Locates point route location with the specified route identifier.
        /// </summary>
        /// <param name="routeId">The route identifier.</param>
        /// <param name="point">The point.</param>
        /// <param name="error">The error that occured during location.</param>
        /// <returns>
        ///     Returns a <see cref="IGeometry" /> representing the location.
        /// </returns>
        public IGeometry Locate(object routeId, IPoint point, out esriLocatingError error)
        {
            IRouteLocation routeLocation = new RouteMeasurePointLocationClass();
            routeLocation.RouteID = routeId;

            IRouteMeasurePointLocation location = (IRouteMeasurePointLocation)routeLocation;
            location.Measure = point.M;

            IGeometry result;
            this.Locator.Locate(routeLocation, out result, out error);

            return result.IsEmpty ? null : result;
        }

        /// <summary>
        ///     Locates line route location with the specified route identifier.
        /// </summary>
        /// <param name="routeId">The route identifier.</param>
        /// <param name="polyline">The polyline.</param>
        /// <param name="error">The error that occured during location.</param>
        /// <returns>
        ///     Returns a <see cref="IGeometry" /> representing the location.
        /// </returns>
        public IGeometry Locate(object routeId, IPolyline polyline, out esriLocatingError error)
        {
            IRouteLocation routeLocation = new RouteMeasureLineLocationClass();
            routeLocation.RouteID = routeId;

            IMSegmentation segmentation = (IMSegmentation)polyline;

            IRouteMeasureLineLocation lineLocation = (IRouteMeasureLineLocation)routeLocation;
            lineLocation.FromMeasure = segmentation.MMin;
            lineLocation.ToMeasure = segmentation.MMax;

            IGeometry result;
            this.Locator.Locate(routeLocation, out result, out error);

            return result.IsEmpty ? null : result;
        }

        /// <summary>
        ///     Locate point features along the route and  writes the result to a new point event table.
        /// </summary>
        /// <param name="locatePointsAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="points">The points to locate.</param>
        /// <param name="searchRadius">
        ///     If the input features are points, the search radius is a numeric value defining how far around each point a search
        ///     will be done to find a target route. If the input features are lines, the search tolerance is really a cluster
        ///     tolerance, which is a numeric value
        ///     representing the maximum tolerated distance between the input lines and the target routes. If the input features
        ///     are polygons, this parameter is ignored since no search radius is used.
        /// </param>
        /// <param name="searchMultipleLocations">
        ///     If the point falls on more than one route for the given search radius, you can
        ///     have the option to create multiple event records that correspond to each route in the search radius vicinity
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the point data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="workspace">The workspace that will contain the event data table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePointsAlongRouteName, IFeatureClass points, double searchRadius, bool searchMultipleLocations, IQueryFilter filter, bool keepAllFields, IWorkspace workspace)
        {
            var properties = new RouteMeasurePointPropertiesClass();
            properties.EventRouteIDFieldName = this.Name.RouteIDFieldName;
            properties.MeasureFieldName = "MEASURE";

            return this.Locate(locatePointsAlongRouteName, points, searchRadius, searchMultipleLocations, properties, filter, keepAllFields, workspace);
        }

        /// <summary>
        ///     Locate point features along the route and  writes the result to a new point event table.
        /// </summary>
        /// <param name="locatePointsAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="points">The points to locate.</param>
        /// <param name="searchRadius">
        ///     If the input features are points, the search radius is a numeric value defining how far around each point a search
        ///     will be done to find a target route. If the input features are lines, the search tolerance is really a cluster
        ///     tolerance, which is a numeric value
        ///     representing the maximum tolerated distance between the input lines and the target routes. If the input features
        ///     are polygons, this parameter is ignored since no search radius is used.
        /// </param>
        /// <param name="searchMultipleLocations">
        ///     If the point falls on more than one route for the given search radius, you can
        ///     have the option to create multiple event records that correspond to each route in the search radius vicinity
        /// </param>
        /// <param name="properties">
        ///     Parameter consisting of the route location fields and the type of events that will be written
        ///     to the output event table.
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the point data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="workspace">The workspace that will contain the event data table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePointsAlongRouteName, IFeatureClass points, double searchRadius, bool searchMultipleLocations, IRouteMeasurePointProperties properties, IQueryFilter filter, bool keepAllFields, IWorkspace workspace)
        {
            var locator = this.GetRouteLocatorOperations(filter, points);

            var outputClassName = workspace.Define(locatePointsAlongRouteName, new TableNameClass());
            workspace.Delete(outputClassName);

            return locator.LocatePointFeatures(searchRadius, searchMultipleLocations, (IRouteEventProperties)properties, keepAllFields, outputClassName, "", null);
        }

        /// <summary>
        ///     Locate line features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locateLinesAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="lines">The lines to locate.</param>
        /// <param name="clusterTolerance">
        ///     The cluster tolerance which is a numeric value
        ///     representing the maximum tolerated distance between the input lines and the target routes.
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the line data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="workspace">The workspace that will contain the event data table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locateLinesAlongRouteName, IFeatureClass lines, double clusterTolerance, IQueryFilter filter, bool keepAllFields, IWorkspace workspace)
        {
            var properties = new RouteMeasureLinePropertiesClass();
            properties.EventRouteIDFieldName = this.Name.RouteIDFieldName;
            properties.ToMeasureFieldName = "TMEAS";
            properties.FromMeasureFieldName = "FMEAS";

            return this.Locate(locateLinesAlongRouteName, lines, clusterTolerance, properties, filter, keepAllFields, workspace);
        }

        /// <summary>
        ///     Locate line features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locateLinesAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="lines">The lines to locate.</param>
        /// <param name="clusterTolerance">
        ///     The cluster tolerance which is a numeric value
        ///     representing the maximum tolerated distance between the input lines and the target routes.
        /// </param>
        /// <param name="properties">
        ///     Parameter consisting of the route location fields and the type of events that will be written
        ///     to the output event table.
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the line data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="workspace">The workspace that will contain the event data table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locateLinesAlongRouteName, IFeatureClass lines, double clusterTolerance, IRouteMeasureLineProperties properties, IQueryFilter filter, bool keepAllFields, IWorkspace workspace)
        {
            var locator = this.GetRouteLocatorOperations(filter, lines);

            var outputClassName = workspace.Define(locateLinesAlongRouteName, new TableNameClass());
            workspace.Delete(outputClassName);

            return locator.LocateLineFeatures(clusterTolerance, (IRouteEventProperties)properties, keepAllFields, outputClassName, "", null);
        }

        /// <summary>
        ///     Locates the polygon features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locatePolygonsAlongRouteName">Name of the locate polygons along route.</param>
        /// <param name="polygons">The polygons to locate.</param>
        /// <param name="keepZeroLengthEvents">
        ///     if set to <c>true</c> allows you to keep or not keep the zero length line events in
        ///     the output event table.  The zero length line events result from a case where the geometric intersection of the
        ///     route (line) and the polygon is a point.
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the polygon data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="workspace">The workspace that will contain the event data table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePolygonsAlongRouteName, IFeatureClass polygons, bool keepZeroLengthEvents, IQueryFilter filter, bool keepAllFields, IWorkspace workspace)
        {
            var properties = new RouteMeasureLinePropertiesClass();
            properties.EventRouteIDFieldName = this.Name.RouteIDFieldName;
            properties.ToMeasureFieldName = "TMEASURE";
            properties.FromMeasureFieldName = "FMEASURE";

            return this.Locate(locatePolygonsAlongRouteName, polygons, keepZeroLengthEvents, properties, filter, keepAllFields, workspace);
        }

        /// <summary>
        ///     Locates the polygon features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="locatePolygonsAlongRouteName">Name of the locate polygons along route.</param>
        /// <param name="polygons">The polygons to locate.</param>
        /// <param name="keepZeroLengthEvents">
        ///     if set to <c>true</c> allows you to keep or not keep the zero length line events in
        ///     the output event table.  The zero length line events result from a case where the geometric intersection of the
        ///     route (line) and the polygon is a point.
        /// </param>
        /// <param name="properties">
        ///     Parameter consisting of the route location fields and the type of events that will be written
        ///     to the output event table.
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the polygon data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="workspace">The workspace that will contain the event data table.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public ITable Locate(string locatePolygonsAlongRouteName, IFeatureClass polygons, bool keepZeroLengthEvents, IRouteMeasureLineProperties properties, IQueryFilter filter, bool keepAllFields, IWorkspace workspace)
        {
            var locator = this.GetRouteLocatorOperations(filter, polygons);

            var outputClassName = workspace.Define(locatePolygonsAlongRouteName, new TableNameClass());
            workspace.Delete(outputClassName);

            return locator.LocatePolygonFeatures((IRouteEventProperties)properties, keepAllFields, keepZeroLengthEvents, outputClassName, "", null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Initializes the route locator for the route.
        /// </summary>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cred="IRouteLocatorOperations2" /> represents the route locator.
        /// </returns>
        private IRouteLocatorOperations2 GetRouteLocatorOperations(IQueryFilter filter, IFeatureClass source)
        {
            IRouteLocatorOperations2 locator = new RouteLocatorOperationsClass();
            locator.RouteLocator = this.Locator;
            locator.RouteIDFieldName = this.Locator.RouteIDFieldName;

            if (source.HasOID && (filter != null && !string.IsNullOrEmpty(filter.WhereClause)))
            {
                IScratchWorkspaceFactory2 factory = new ScratchWorkspaceFactoryClass();
                var selectionContainer = factory.DefaultScratchWorkspace;

                var selection = source.Select(filter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, selectionContainer);
                locator.InputFeatureSelection = selection;
            }
            else
            {
                locator.InputFeatureClass = source;
            }

            return locator;
        }

        /// <summary>
        ///     Gets the name of the route measure locator.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="routeIDFieldName">Name of the route identifier field.</param>
        /// <param name="routeMeasureUnit">The route measure unit.</param>
        /// <param name="routeIDIsUnique">if set to <c>true</c> the route identifier is unique.</param>
        /// <param name="routeWhereClause">The route where clause.</param>
        /// <returns></returns>
        private IRouteLocatorName GetRouteMeasureLocatorName(IFeatureClass source, string routeIDFieldName, esriUnits routeMeasureUnit, bool routeIDIsUnique, string routeWhereClause)
        {
            IRouteLocatorName locatorName = new RouteMeasureLocatorNameClass();
            locatorName.RouteFeatureClassName = ((IDataset)source).FullName;
            locatorName.RouteMeasureUnit = routeMeasureUnit;
            locatorName.RouteIDFieldName = routeIDFieldName;
            locatorName.RouteIDIsUnique = routeIDIsUnique;
            locatorName.RouteWhereClause = routeWhereClause ?? "";

            return locatorName;
        }

        #endregion
    }

    /// <summary>
    /// </summary>
    public class RouteLocationResult
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteLocationResult" /> class.
        /// </summary>
        /// <param name="location">Describes a portion of a route or a single position along a route.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="error">The error.</param>
        public RouteLocationResult(IRouteLocation location, IFeature feature, IGeometry geometry, esriLocatingError error)
        {
            this.Location = location;
            this.Feature = feature;
            this.Geometry = geometry;
            this.Error = error;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the error.
        /// </summary>
        /// <value>
        ///     The error.
        /// </value>
        public esriLocatingError Error { get; private set; }

        /// <summary>
        ///     Gets the feature corresponding to the location.
        /// </summary>
        /// <value>
        ///     The feature.
        /// </value>
        public IFeature Feature { get; private set; }

        /// <summary>
        ///     Gets the geometry of the route location.
        /// </summary>
        /// <value>
        ///     The geometry.
        /// </value>
        public IGeometry Geometry { get; private set; }

        /// <summary>
        ///     Gets the portion of a route or a single position along a route.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        public IRouteLocation Location { get; private set; }

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

            var outputClassName = workspace.Define(outputTableName, new FeatureClassNameClass());
            workspace.Delete(outputClassName);

            var dataConverter = new FeatureDataConverterClass();
            var invalidObjects = dataConverter.ConvertFeatureClass((IDatasetName)this.Name, filter, null, null, outputClassName, geometryDef, fields, "", 1000, 0);
            if (invalidObjects.AsEnumerable().Any()) return null;

            var route = workspace.GetFeatureClass(outputTableName);
            return route;
        }

        #endregion
    }
}