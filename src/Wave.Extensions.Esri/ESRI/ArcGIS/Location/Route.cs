using System.Collections.Generic;

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
        /// <param name="segmentation">The linear dynamic segmentation properties.</param>
        public Route(IFeatureClass routeFeatureClass, IRouteMeasureLineSegmentation segmentation)
        {
            this.FeatureClass = routeFeatureClass;
            this.Segmentation = segmentation;
            this.Errors = new List<string>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Route" /> class.
        /// </summary>
        /// <param name="eventData">The event route data.</param>
        /// <param name="segmentation">The linear dynamic segmentation properties.</param>
        /// <param name="workspace">The workpace that contains the route event data.</param>
        public Route(EventData eventData, IRouteMeasureLineSegmentation segmentation, IWorkspace workspace)
        {
            var source = workspace.GetFeatureClass("", eventData.EventTableName);
            var filter = new QueryFilterClass {WhereClause = eventData.WhereClause};

            this.Errors = this.Create(eventData.OutputTableName, source, segmentation, workspace, filter, null);
            this.FeatureClass = workspace.GetFeatureClass("", eventData.EventTableName);
            this.Segmentation = segmentation;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Route" /> class.
        /// </summary>
        /// <param name="outputRouteName">The name of the output route feature class.</param>
        /// <param name="source">The source (linear) feature class.</param>
        /// <param name="segmentation">The linear dynamic segmentation properties.</param>
        /// <param name="outputWorkspace">The location of the output route feature class.</param>
        /// <param name="filter">The filter used to query the source feature class for route data.</param>
        /// <param name="trackCancel">The object used to monitor the progress of the actions.</param>
        public Route(string outputRouteName, IFeatureClass source, IRouteMeasureLineSegmentation segmentation, IWorkspace outputWorkspace, IQueryFilter filter, ITrackCancel trackCancel)
        {
            this.Errors = this.Create(outputRouteName, source, segmentation, outputWorkspace, filter, trackCancel);
            this.FeatureClass = outputWorkspace.GetFeatureClass("", outputRouteName);
            this.Segmentation = segmentation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the errors that may have occured when creating the route.
        /// </summary>
        public IEnumerable<string> Errors { get; private set; }

        /// <summary>
        ///     Gets the route feature class that was created.
        /// </summary>
        public IFeatureClass FeatureClass { get; private set; }

        /// <summary>
        ///     Gets the route locator.
        /// </summary>
        public IRouteLocator2 Locator
        {
            get
            {
                IRouteLocatorName locatorName = new RouteMeasureLocatorNameClass();
                locatorName.RouteFeatureClassName = ((IDataset) this.FeatureClass).FullName;
                locatorName.RouteMeasureUnit = this.Segmentation.RouteEventProperties.EventMeasureUnit;
                locatorName.RouteIDFieldName = this.Segmentation.RouteIDFieldName;
                locatorName.RouteIDIsUnique = false;

                return ((IName) locatorName).Open() as IRouteLocator2;
            }
        }

        /// <summary>
        ///     Gets the linear segmentation properties.
        /// </summary>
        public IRouteMeasureLineSegmentation Segmentation { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Locate point features along the route and write the result in an object class.
        /// </summary>
        /// <param name="locateAlongRouteName">The name of the event table of the located features.</param>
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
        /// <param name="segmentation">
        ///     Parameter consisting of the route location fields and the type of events that will be written
        ///     to the output event table.
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        public ITable Locate(string locateAlongRouteName, double searchRadius, bool searchMultipleLocations, IRouteMeasurePointSegmentation segmentation, IQueryFilter filter, bool keepAllFields)
        {
            var locator = this.GetRouteLocatorOperations(filter);

            var workspace = ((IDataset) this.FeatureClass).Workspace;

            var outputClassName = new TableNameClass();
            outputClassName.WorkspaceName = (IWorkspaceName) ((IDataset) workspace).FullName;
            outputClassName.Name = locateAlongRouteName;

            workspace.Delete(outputClassName);

            return locator.LocatePointFeatures(searchRadius, searchMultipleLocations, segmentation.RouteEventProperties, keepAllFields, outputClassName, "", null);
        }

        /// <summary>
        ///     Locate lines features along the route and write the result in an object class.
        /// </summary>
        /// <param name="locateAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="clusterTolerance">
        ///     The cluster tolerance which is a numeric value
        ///     representing the maximum tolerated distance between the input lines and the target routes.
        /// </param>
        /// <param name="segmentation">
        ///     Parameter consisting of the route location fields and the type of events that will be written
        ///     to the output event table.
        /// </param>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        public ITable Locate(string locateAlongRouteName, double clusterTolerance, IRouteMeasureLineSegmentation segmentation, IQueryFilter filter, bool keepAllFields)
        {
            var locator = this.GetRouteLocatorOperations(filter);

            var workspace = ((IDataset) this.FeatureClass).Workspace;

            var outputClassName = new TableNameClass();
            outputClassName.WorkspaceName = (IWorkspaceName) ((IDataset) workspace).FullName;
            outputClassName.Name = locateAlongRouteName;

            workspace.Delete(outputClassName);

            return locator.LocateLineFeatures(clusterTolerance, segmentation.RouteEventProperties, keepAllFields, outputClassName, "", null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates a route feature class from a layer based on a polyline feature class.
        /// </summary>
        /// <param name="outputRouteName">The name of the output route feature class.</param>
        /// <param name="source">The source (linear) feature class.</param>
        /// <param name="linear">The linear dynamic segmentation properties.</param>
        /// <param name="workspace">The location of the output route feature class.</param>
        /// <param name="filter">The filter used to query the source feature class for route data.</param>
        /// <param name="trackCancel">The object used to monitor the progress of the actions.</param>
        /// <returns>Returns a <see cref="IEnumerable{String}" /> representing the errors of the routes that could not be created.</returns>
        private IEnumerable<string> Create(string outputRouteName, IFeatureClass source, IRouteMeasureLineSegmentation linear, IWorkspace workspace, IQueryFilter filter, ITrackCancel trackCancel)
        {
            IRouteMeasureCreator2 gp = new RouteMeasureCreatorClass();
            gp.InputRouteIDFieldName = linear.RouteIDFieldName;

            if (source.HasOID)
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

            var outputClassName = new FeatureClassNameClass();
            outputClassName.WorkspaceName = (IWorkspaceName) ((IDataset) workspace).FullName;
            outputClassName.Name = outputRouteName;

            workspace.Delete(outputClassName);

            var i = source.Fields.FindField(source.ShapeFieldName);
            var field = source.Fields.Field[i];
            var clone = (IClone) field.GeometryDef;
            var geometryDef = (IGeometryDef) clone.Clone();

            var errors = gp.CreateUsing2Fields(linear.FromMeasureFieldName, linear.ToMeasureFieldName, outputClassName, geometryDef, "", trackCancel);
            return errors.AsEnumerable();
        }

        /// <summary>
        ///     Initializes the route locator for the route.
        /// </summary>
        /// <param name="filter">An optional filter used to use a subset of the route data.</param>
        /// <returns>Returns a <see cred="IRouteLocatorOperations2" /> represents the route locator.</returns>
        private IRouteLocatorOperations2 GetRouteLocatorOperations(IQueryFilter filter)
        {
            IRouteLocatorOperations2 locator = new RouteLocatorOperationsClass();
            locator.RouteIDFieldName = this.Segmentation.RouteIDFieldName;
            locator.RouteLocator = this.Locator;

            if (this.FeatureClass.HasOID)
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
    ///     A route layer is useful for determining the best route between a set of locations based on a specified network
    ///     cost.
    /// </summary>
    public sealed class RouteLayer
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteLayer" /> class.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="route">The route.</param>
        /// <param name="segmentation">The segmentation.</param>
        /// <param name="workspace">The workspace.</param>
        public RouteLayer(EventData eventData, Route route, IRouteMeasureLineSegmentation segmentation, IWorkspace workspace)
            : this(eventData, route.FeatureClass, segmentation, workspace)
        {
            this.Route = route;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteLayer" /> class.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="workspace">The workspace.</param>
        public RouteLayer(LayerRouteEventData eventData, IWorkspace workspace)
            : this(eventData, workspace.GetFeatureClass("", eventData.Route), eventData.Segmentation, workspace)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RouteLayer" /> class.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="route">The route.</param>
        /// <param name="segmentation">The segmentation.</param>
        /// <param name="workspace">The workspace.</param>
        public RouteLayer(EventData eventData, IFeatureClass route, IRouteMeasureLineSegmentation segmentation, IWorkspace workspace)
        {
            var eventTable = workspace.GetTable("", eventData.EventTableName);
            var filter = new QueryFilterClass {WhereClause = eventData.WhereClause};

            this.Errors = this.Create(eventTable, route, segmentation, eventData.OutputTableName, filter, workspace);
            this.FeatureClass = workspace.GetFeatureClass("", eventData.OutputTableName);
            this.Route = new Route(route, segmentation);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the errors.
        /// </summary>
        /// <value>
        ///     The errors.
        /// </value>
        public IEnumerable<IInvalidObjectInfo> Errors { get; private set; }

        /// <summary>
        ///     Gets the feature class.
        /// </summary>
        /// <value>
        ///     The feature class.
        /// </value>
        public IFeatureClass FeatureClass { get; private set; }

        /// <summary>
        ///     Gets the route.
        /// </summary>
        /// <value>
        ///     The route.
        /// </value>
        public Route Route { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates the route event layer.
        /// </summary>
        /// <param name="eventTable">The event table that contains the event data.</param>
        /// <param name="route">The route linear feature class that will be used as the source.</param>
        /// <param name="segmentation">The linear segmentation properties.</param>
        /// <param name="outputTableName">The name of the output feature class.</param>
        /// <param name="filter">The filter used to limit the number of features.</param>
        /// <param name="workspace">The workspace that will contain the newly created feature class layer.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IInvalidObjectInfo}" /> representing the errors occured when creating route
        ///     feature class.
        /// </returns>
        private IEnumerable<IInvalidObjectInfo> Create(ITable eventTable, IFeatureClass route, IRouteMeasureSegmentation segmentation, string outputTableName, IQueryFilter filter, IWorkspace workspace)
        {
            IRouteLocatorName locatorName = new RouteMeasureLocatorNameClass();
            locatorName.RouteFeatureClassName = ((IDataset) route).FullName;
            locatorName.RouteMeasureUnit = segmentation.RouteEventProperties.EventMeasureUnit;
            locatorName.RouteIDFieldName = segmentation.RouteIDFieldName;
            locatorName.RouteIDIsUnique = false;

            IRouteEventSourceName eventSourceName = new RouteEventSourceNameClass();
            eventSourceName.EventProperties = segmentation.RouteEventProperties;
            eventSourceName.EventTableName = ((IDataset) eventTable).FullName;
            eventSourceName.RouteLocatorName = locatorName;

            var outputClassName = new FeatureClassNameClass();
            outputClassName.WorkspaceName = (IWorkspaceName) ((IDataset) workspace).FullName;
            outputClassName.Name = outputTableName;

            workspace.Delete(outputClassName);

            IRouteEventSource eventSource = (IRouteEventSource) ((IName) eventSourceName).Open();            
            IFeatureClass sourceClass = (IFeatureClass) eventSource;

            var i = route.Fields.FindField(route.ShapeFieldName);
            var field = route.Fields.Field[i];
            var clone = (IClone) field.GeometryDef;
            var geometryDef = (IGeometryDef) clone.Clone();

            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldErrors;
            IFields outputFields;

            fieldChecker.InputWorkspace = workspace;
            fieldChecker.ValidateWorkspace = workspace;
            fieldChecker.Validate(sourceClass.Fields, out enumFieldErrors, out outputFields);

            var dataConverter = new FeatureDataConverterClass();
            var invalidObjects = dataConverter.ConvertFeatureClass((IDatasetName) eventSourceName, filter, null, null, outputClassName, geometryDef, outputFields, "", 1000, 0);
            return invalidObjects.AsEnumerable();
        }

        #endregion
    }
}