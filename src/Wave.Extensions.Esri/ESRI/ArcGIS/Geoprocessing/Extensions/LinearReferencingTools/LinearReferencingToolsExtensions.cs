using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.LinearReferencingTools;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the Linea rReferencing Geoprocesing Tools
    /// </summary>
    public static class LinearReferencingToolsExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Locate lines features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="source">The lines to locate.</param>
        /// <param name="locateLinesAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="route">The route to locate.</param>
        /// <param name="routeIDFieldName">
        ///     The field containing values that uniquely identify each route. This field can be numeric
        ///     or character.
        /// </param>
        /// <param name="clusterTolerance">
        ///     A numeric value representing the maximum tolerated distance between the input lines and the target routes.
        /// </param>
        /// <param name="toleranceUnits">The units.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public static ITable LocateAlongRoute(this IFeatureClass source, string locateLinesAlongRouteName, IFeatureClass route, string routeIDFieldName, double clusterTolerance, esriUnits toleranceUnits, bool keepAllFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return LocateAlongRouteImpl(source, locateLinesAlongRouteName, ((IDataset) source).Workspace, null, route, null, routeIDFieldName, clusterTolerance, toleranceUnits, false, keepAllFields, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Locate lines features along the route and writes the result to a new line event table.
        /// </summary>
        /// <param name="source">The lines to locate.</param>
        /// <param name="locateLinesAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="sourceWhereClause">The source where clause.</param>
        /// <param name="route">The route to locate.</param>
        /// <param name="routeWhereClause">The route where clause.</param>
        /// <param name="routeIDFieldName">
        ///     The field containing values that uniquely identify each route. This field can be numeric
        ///     or character.
        /// </param>
        /// <param name="clusterTolerance">
        ///     A numeric value representing the maximum tolerated distance between the input lines and
        ///     the target routes.
        /// </param>
        /// <param name="toleranceUnits">The units.</param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public static ITable LocateAlongRoute(this IFeatureClass source, string locateLinesAlongRouteName, IWorkspace workspace, string sourceWhereClause, IFeatureClass route, string routeWhereClause, string routeIDFieldName, double clusterTolerance, esriUnits toleranceUnits, bool keepAllFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return LocateAlongRouteImpl(source, locateLinesAlongRouteName, workspace, sourceWhereClause, route, routeWhereClause, routeIDFieldName, clusterTolerance, toleranceUnits, false, keepAllFields, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Locate point features along the route and writes the result to a new point event table.
        /// </summary>
        /// <param name="source">The points to locate.</param>
        /// <param name="locatePointsAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="route">The route to locate.</param>
        /// <param name="routeIDFieldName">
        ///     The field containing values that uniquely identify each route. This field can be numeric
        ///     or character.
        /// </param>
        /// <param name="searchRadius">
        ///     If the input features are points, the search radius is a numeric value defining how far around each point a search
        ///     will be done to find a target route. If the input features are lines, the search tolerance is really a cluster
        ///     tolerance, which is a numeric value
        ///     representing the maximum tolerated distance between the input lines and the target routes. If the input features
        ///     are polygons, this parameter is ignored since no search radius is used.
        /// </param>
        /// <param name="units">The units.</param>
        /// <param name="searchMultipleLocations">
        ///     If the point falls on more than one route for the given search radius, you can
        ///     have the option to create multiple event records that correspond to each route in the search radius vicinity
        /// </param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public static ITable LocateAlongRoute(this IFeatureClass source, string locatePointsAlongRouteName, IFeatureClass route, string routeIDFieldName, double searchRadius, esriUnits units, bool searchMultipleLocations, bool keepAllFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return source.LocateAlongRoute(locatePointsAlongRouteName, ((IDataset) source).Workspace, null, route, null, routeIDFieldName, searchRadius, units, searchMultipleLocations, keepAllFields, trackCancel, eventHandler);
        }

        /// <summary>
        ///     Locate point features along the route and writes the result to a new point event table.
        /// </summary>
        /// <param name="source">The points to locate.</param>
        /// <param name="locatePointsAlongRouteName">The name of the event table of the located features.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="sourceWhereClause">An optional filter used to use a subset of the points data.</param>
        /// <param name="route">The route to locate.</param>
        /// <param name="routeWhereClause">An optional filter used to use a subset of the route data.</param>
        /// <param name="routeIDFieldName">
        ///     The field containing values that uniquely identify each route. This field can be numeric
        ///     or character.
        /// </param>
        /// <param name="searchRadius">
        ///     If the input features are points, the search radius is a numeric value defining how far around each point a search
        ///     will be done to find a target route. If the input features are lines, the search tolerance is really a cluster
        ///     tolerance, which is a numeric value
        ///     representing the maximum tolerated distance between the input lines and the target routes. If the input features
        ///     are polygons, this parameter is ignored since no search radius is used.
        /// </param>
        /// <param name="units">The units.</param>
        /// <param name="searchMultipleLocations">
        ///     If the point falls on more than one route for the given search radius, you can
        ///     have the option to create multiple event records that correspond to each route in the search radius vicinity
        /// </param>
        /// <param name="keepAllFields">
        ///     Allows you to include or disinclude the attributes of the point feature class.  If this is
        ///     set to False, the output event table will only contain the route event properties.
        /// </param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     Returns a <see cref="ITable" /> representing the event table of the results.
        /// </returns>
        public static ITable LocateAlongRoute(this IFeatureClass source, string locatePointsAlongRouteName, IWorkspace workspace, string sourceWhereClause, IFeatureClass route, string routeWhereClause, string routeIDFieldName, double searchRadius, esriUnits units, bool searchMultipleLocations, bool keepAllFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return LocateAlongRouteImpl(source, locatePointsAlongRouteName, workspace, sourceWhereClause, route, routeWhereClause, routeIDFieldName, searchRadius, units, searchMultipleLocations, keepAllFields, trackCancel, eventHandler);
        }

        #endregion

        #region Private Methods

       
        /// <summary>
        ///     Locates the along route implementation.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="locateAlongRouteName">Name of the locate along route.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="sourceWhereClause">The source where clause.</param>
        /// <param name="route">The route.</param>
        /// <param name="routeWhereClause">The route where clause.</param>
        /// <param name="routeIDFieldName">Name of the route identifier field.</param>
        /// <param name="radiusOrTolerance">The radius or tolerance.</param>
        /// <param name="radiusOrToleranceUnits">The radius or tolerance units.</param>
        /// <param name="searchMultipleLocations">if set to <c>true</c> [search multiple locations].</param>
        /// <param name="keepAllFields">if set to <c>true</c> [keep all fields].</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns></returns>
        private static ITable LocateAlongRouteImpl(IFeatureClass source, string locateAlongRouteName, IWorkspace workspace, string sourceWhereClause, IFeatureClass route, string routeWhereClause, string routeIDFieldName, double radiusOrTolerance, esriUnits radiusOrToleranceUnits, bool searchMultipleLocations, bool keepAllFields, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            var ds = (IDataset) source;

            object input = route;
            object features = source;

            if (!string.IsNullOrEmpty(routeWhereClause))
            {
                var name = string.Format("{0}_V", ((IDataset) route).Name);
                input = route.MakeView(name, routeWhereClause, null, trackCancel, eventHandler);
            }

            if (!string.IsNullOrEmpty(sourceWhereClause))
            {
                var name = string.Format("{0}_V", ds.Name);
                features = source.MakeView(name, sourceWhereClause, null, trackCancel, eventHandler);
            }

            LocateFeaturesAlongRoutes gp = new LocateFeaturesAlongRoutes();
            gp.in_features = features;
            gp.in_fields = keepAllFields ? "FIELDS" : "NO_FIELDS";
            gp.in_routes = input;
            gp.route_id_field = routeIDFieldName;

            gp.out_table = workspace.GetAbsolutePath(locateAlongRouteName);

            if (source.ShapeType == esriGeometryType.esriGeometryPolyline)
            {
                gp.out_event_properties = string.Format("{0} LINE FMEASURE TMEASURE", routeIDFieldName);
            }
            else
            {
                gp.out_event_properties = string.Format("{0} POINT MEASURE", routeIDFieldName);
            }

            IUnitConverter converter = new UnitConverterClass();
            gp.radius_or_tolerance = string.Format("{0} {1}", radiusOrTolerance, converter.EsriUnitsAsString(radiusOrToleranceUnits, esriCaseAppearance.esriCaseAppearanceUpper, true));

            gp.route_locations = searchMultipleLocations ? "ALL" : "FIRST";

            if (gp.Run(trackCancel, eventHandler) == esriJobStatus.esriJobSucceeded)
                return workspace.GetTable(locateAlongRouteName);

            return null;
        }

        #endregion
    }
}