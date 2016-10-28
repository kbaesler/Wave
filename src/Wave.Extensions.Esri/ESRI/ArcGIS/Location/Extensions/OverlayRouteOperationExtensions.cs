using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Provides extension methods for overlaying two event tables to create an output event table that represents the
    ///     union or intersection of the input.
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteOperation{OverlayRouteEventData}" />
    /// <remarks>
    ///     Line-on-line, line-on-point, point-on-line, and point-on-point event overlays can be performed.
    ///     The input and overlay events should be based on the same route reference.
    /// </remarks>
    public static class OverlayRouteOperationExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Overlays two event tables to create an output event table that represents the union or intersection of the input.
        /// </summary>
        /// <param name="sourceSelection">The input event table selection.</param>
        /// <param name="source">
        ///     Parameter consisting of the route location fields and the type of events in the input event
        ///     table.
        /// </param>
        /// <param name="overlaySelection">The overlay event table selection.</param>
        /// <param name="overlay">
        ///     Parameter consisting of the route location fields and the type of events in the overlay event
        ///     table.
        /// </param>
        /// <param name="type">The type of overlay to be performed.</param>
        /// <param name="output">
        ///     Parameter consisting of the route location fields and the type of events in the overlay event
        ///     table.
        /// </param>
        /// <param name="outputTableName">The table to be created.</param>
        /// <param name="outputWorkspace">The workspace that will contain the table that has been created.</param>
        /// <param name="trackCancel">Allows the operation be be cancelled.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        public static ITable Overlay(this ISelectionSet sourceSelection, IRouteMeasureSegmentation source, ISelectionSet overlaySelection, IRouteMeasureSegmentation overlay, OverlayType type, IRouteMeasureSegmentation output, string outputTableName, IWorkspace outputWorkspace, ITrackCancel trackCancel)
        {
            IRouteMeasureEventGeoprocessor2 gp = new RouteMeasureGeoprocessorClass();
            gp.InputEventProperties = source.EventProperties;
            gp.InputSelection = sourceSelection;
            gp.BuildOutputIndex = true;
            gp.OverlayEventProperties = overlay.EventProperties;
            gp.OverlaySelection = overlaySelection;
            gp.KeepZeroLengthLineEvents = false;

            return OverlayImpl(gp, type, output, outputTableName, outputWorkspace, trackCancel);
        }

        /// <summary>
        ///     Overlays two event tables to create an output event table that represents the union or intersection of the input.
        /// </summary>
        /// <param name="sourceTable">The input event table.</param>
        /// <param name="source">
        ///     Parameter consisting of the route location fields and the type of events in the input event
        ///     table.
        /// </param>
        /// <param name="overlayTable">The overlay event table.</param>
        /// <param name="overlay">
        ///     Parameter consisting of the route location fields and the type of events in the overlay event
        ///     table.
        /// </param>
        /// <param name="type">The type of overlay to be performed.</param>
        /// <param name="output">
        ///     Parameter consisting of the route location fields and the type of events in the overlay event
        ///     table.
        /// </param>
        /// <param name="outputTableName">The table to be created.</param>
        /// <param name="outputWorkspace">The workspace that will contain the table that has been created.</param>
        /// <param name="trackCancel">Allows the operation be be cancelled.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        public static ITable Overlay(this ITable sourceTable, IRouteMeasureSegmentation source, ITable overlayTable, IRouteMeasureSegmentation overlay, OverlayType type, IRouteMeasureSegmentation output, string outputTableName, IWorkspace outputWorkspace, ITrackCancel trackCancel)
        {
            IRouteMeasureEventGeoprocessor2 gp = new RouteMeasureGeoprocessorClass();
            gp.InputEventProperties = source.EventProperties;
            gp.InputTable = sourceTable;
            gp.BuildOutputIndex = true;
            gp.OverlayEventProperties = overlay.EventProperties;
            gp.OverlayTable = overlayTable;
            gp.KeepZeroLengthLineEvents = false;

            return OverlayImpl(gp, type, output, outputTableName, outputWorkspace, trackCancel);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Overlays two event tables to create an output event table that represents the union or intersection of the input.
        /// </summary>
        /// <param name="gp">The route event geoprocessor.</param>
        /// <param name="type">The type of overlay to be performed.</param>
        /// <param name="output">
        ///     Parameter consisting of the route location fields and the type of events in the overlay event
        ///     table.
        /// </param>
        /// <param name="outputTableName">The table to be created.</param>
        /// <param name="outputWorkspace">The workspace that will contain the table that has been created.</param>
        /// <param name="trackCancel">Allows the operation be be cancelled.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        private static ITable OverlayImpl(IRouteMeasureEventGeoprocessor2 gp, OverlayType type, IRouteMeasureSegmentation output, string outputTableName, IWorkspace outputWorkspace, ITrackCancel trackCancel)
        {
            var outputName = new TableNameClass();
            outputName.WorkspaceName = (IWorkspaceName) ((IDataset) outputWorkspace).FullName;
            outputName.Name = outputTableName;

            outputWorkspace.Delete(outputName);

            if (type == OverlayType.Union)
            {
                return gp.Union2(output.EventProperties, true, outputName, trackCancel, "");
            }

            return gp.Intersect2(output.EventProperties, true, outputName, trackCancel, "");
        }

        #endregion
    }
}