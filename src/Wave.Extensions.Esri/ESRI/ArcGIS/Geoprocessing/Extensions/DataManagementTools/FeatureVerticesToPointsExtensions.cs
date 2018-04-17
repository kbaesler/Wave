using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the <see cref="FeatureVerticesToPoints" /> geoprocessing tool.
    /// </summary>
    public static class FeatureVerticesToPointsExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates a feature class containing two points will be created, one at the start point and another at the endpoint
        ///     of each input feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <remarks>License ArcGIS Desktop Advanced: Yes</remarks>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class.
        /// </returns>
        public static IFeatureClass CreateBothVertexPoints(this IFeatureClass source, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return CreateVertexPointsImpl(source, outputTableName, workspace, trackCancel, eventHandler, "BOTH_ENDS");
        }

        /// <summary>
        ///     Creates a feature class containing dangle points for any start or end point of an input line, if that point is not
        ///     connected to another line at any location along that line. This option does not apply to polygon input.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <remarks>License ArcGIS Desktop Advanced: Yes</remarks>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class.
        /// </returns>
        public static IFeatureClass CreateDangleVertexPoints(this IFeatureClass source, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return CreateVertexPointsImpl(source, outputTableName, workspace, trackCancel, eventHandler, "DANGLE");
        }

        /// <summary>
        ///     Creates a feature class containing points at the start point (first vertex) of each input feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <remarks>License ArcGIS Desktop Advanced: Yes</remarks>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class.
        /// </returns>
        public static IFeatureClass CreateFirstVertexPoints(this IFeatureClass source, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return CreateVertexPointsImpl(source, outputTableName, workspace, trackCancel, eventHandler, "START");
        }

        /// <summary>
        ///     Creates a feature class containing points at the end point (last vertex) of each input feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <remarks>License ArcGIS Desktop Advanced: Yes</remarks>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class.
        /// </returns>
        public static IFeatureClass CreateLastVertexPoints(this IFeatureClass source, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return CreateVertexPointsImpl(source, outputTableName, workspace, trackCancel, eventHandler, "END");
        }

        /// <summary>
        ///     Creates a feature class containing points at the midpoint, not necessarily a vertex, of each input line or polygon
        ///     boundary.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <remarks>License ArcGIS Desktop Advanced: Yes</remarks>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class.
        /// </returns>
        public static IFeatureClass CreateMidVertexPoints(this IFeatureClass source, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return CreateVertexPointsImpl(source, outputTableName, workspace, trackCancel, eventHandler, "MID");
        }

        /// <summary>
        /// Creates a feature class containing all points generated from specified vertices or locations of the input features.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <remarks>License ArcGIS Desktop Advanced: Yes</remarks>
        /// <returns>
        /// Returns a <see cref="IFeatureClass" /> representing the feature class.
        /// </returns>
        public static IFeatureClass CreateAllVertexPoints(this IFeatureClass source, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler)
        {
            return CreateVertexPointsImpl(source, outputTableName, workspace, trackCancel, eventHandler, "ALL");
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Creates a feature class containing points generated from specified vertices or locations of the input features.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="outputTableName">Name of the output table.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <param name="eventHandler">The events.</param>
        /// <param name="location">The location.</param>
        /// <remarks>License ArcGIS Desktop Advanced: Yes</remarks>
        /// <returns>
        ///     Returns a <see cref="IFeatureClass" /> representing the feature class.
        /// </returns>
        private static IFeatureClass CreateVertexPointsImpl(IFeatureClass source, string outputTableName, IWorkspace workspace, ITrackCancel trackCancel, IGeoProcessorEvents eventHandler, string location)
        {
            var tableName = workspace.Define(outputTableName, new FeatureClassNameClass());
            workspace.Delete(tableName);

            FeatureVerticesToPoints gp = new FeatureVerticesToPoints();
            gp.in_features = source;
            gp.out_feature_class = workspace.GetAbsolutePath(outputTableName);
            gp.point_location = location;
            
            var status = gp.Run(trackCancel, eventHandler);
            return status == esriJobStatus.esriJobSucceeded ? workspace.GetFeatureClass(outputTableName) : null;
        }

        #endregion
    }
}