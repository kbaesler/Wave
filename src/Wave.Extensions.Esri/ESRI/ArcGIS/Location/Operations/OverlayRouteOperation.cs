using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Overlays two event tables to create an output event table that represents the union or intersection of the input.
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteOperation{OverlayRouteEventData}" />
    /// <remarks>
    ///     Line-on-line, line-on-point, point-on-line, and point-on-point event overlays can be performed.
    ///     The input and overlay events should be based on the same route reference.
    /// </remarks>
    public class OverlayRouteOperation : RouteOperation<OverlayRouteEventData>
    {
        #region Public Methods

        /// <summary>
        ///     Events will be aggregated wherever there is measure overlap.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="workspace">The workspace that contains the event data.</param>
        /// <param name="trackCancel">The object that allows for monitoring the progress.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        public override ITable Execute(OverlayRouteEventData eventData, IWorkspace workspace, ITrackCancel trackCancel)
        {
            var eventTable = workspace.GetTable("", eventData.EventTableName);
            var overlayTable = workspace.GetTable("", eventData.OverlayTableName);

            if (eventTable.HasOID && overlayTable.HasOID && !string.IsNullOrEmpty(eventData.WhereClause))
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = eventData.WhereClause;

                IScratchWorkspaceFactory2 factory = new ScratchWorkspaceFactoryClass();
                var selectionContainer = factory.DefaultScratchWorkspace;

                ISelectionSet sourceSelection = eventTable.Select(filter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, selectionContainer);
                ISelectionSet overlaySelection = eventTable.Select(filter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, selectionContainer);

                return this.Execute(sourceSelection, eventData.Segmentation, overlaySelection, eventData.OverlaySegmentation, eventData.OverlayType, eventData.Segmentation, eventData.OutputTableName, workspace, trackCancel);
            }

            return this.Execute(eventTable, eventData.Segmentation, overlayTable, eventData.OverlaySegmentation, eventData.OverlayType, eventData.Segmentation, eventData.OutputTableName, workspace, trackCancel);
        }

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
        public ITable Execute(ISelectionSet sourceSelection, IRouteMeasureSegmentation source, ISelectionSet overlaySelection, IRouteMeasureSegmentation overlay, OverlayType type, IRouteMeasureSegmentation output, string outputTableName, IWorkspace outputWorkspace, ITrackCancel trackCancel)
        {
            IRouteMeasureEventGeoprocessor2 gp = new RouteMeasureGeoprocessorClass();
            gp.InputEventProperties = source.RouteEventProperties;
            gp.InputSelection = sourceSelection;
            gp.BuildOutputIndex = true;
            gp.OverlayEventProperties = overlay.RouteEventProperties;
            gp.OverlaySelection = overlaySelection;
            gp.KeepZeroLengthLineEvents = false;

            return this.Overlay(gp, type, output, outputTableName, outputWorkspace, trackCancel);
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
        public ITable Execute(ITable sourceTable, IRouteMeasureSegmentation source, ITable overlayTable, IRouteMeasureSegmentation overlay, OverlayType type, IRouteMeasureSegmentation output, string outputTableName, IWorkspace outputWorkspace, ITrackCancel trackCancel)
        {
            IRouteMeasureEventGeoprocessor2 gp = new RouteMeasureGeoprocessorClass();
            gp.InputEventProperties = source.RouteEventProperties;
            gp.InputTable = sourceTable;
            gp.BuildOutputIndex = true;
            gp.OverlayEventProperties = overlay.RouteEventProperties;
            gp.OverlayTable = overlayTable;
            gp.KeepZeroLengthLineEvents = false;

            return this.Overlay(gp, type, output, outputTableName, outputWorkspace, trackCancel);
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
        /// <param name="outputTableName">The table to be created.</param>
        /// <param name="outputWorkspace">The workspace that will contain the table that has been created.</param>
        /// <param name="trackCancel">Allows the operation be be cancelled.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        public ITable Execute(ITable sourceTable, IRouteMeasureSegmentation source, ITable overlayTable, IRouteMeasureSegmentation overlay, OverlayType type, string outputTableName, IWorkspace outputWorkspace, ITrackCancel trackCancel)
        {
            var output = this.GetOutputSegmentation(source, overlay, type);

            return this.Execute(sourceTable, source, overlayTable, overlay, type, output, outputTableName, outputWorkspace, trackCancel);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Returns the output segementation based on the source and overlay segementations (and the type of overlay being
        ///     performed).
        /// </summary>
        /// <param name="source">The source segmentation.</param>
        /// <param name="overlay">The overlay segmentation.</param>
        /// <param name="type">The type of overlay to be performed.</param>
        /// <returns></returns>
        /// <remarks>
        ///     If either the input or overlay event properties are type POINT, the output event properties must be defined as type
        ///     POINT when an INTERSECT overlay is performed.
        ///     The output event properties must be defined as type LINE when a UNION overlay is performed.
        /// </remarks>
        private IRouteMeasureSegmentation GetOutputSegmentation(IRouteMeasureSegmentation source, IRouteMeasureSegmentation overlay, OverlayType type)
        {
            var segmentations = new List<IRouteMeasureSegmentation>();
            segmentations.Add(source);
            segmentations.Add(overlay);

            var points = segmentations.OfType<IRouteMeasurePointSegmentation>().ToArray();
            if (type == OverlayType.Intersect && points.Any())
            {
                return points.First();
            }

            var lines = segmentations.OfType<IRouteMeasureLineSegmentation>().ToArray();
            if (type == OverlayType.Union)
            {
                return lines.First();
            }

            return overlay;
        }

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
        private ITable Overlay(IRouteMeasureEventGeoprocessor2 gp, OverlayType type, IRouteMeasureSegmentation output, string outputTableName, IWorkspace outputWorkspace, ITrackCancel trackCancel)
        {
            var outputName = new TableNameClass();
            outputName.WorkspaceName = (IWorkspaceName) ((IDataset) outputWorkspace).FullName;
            outputName.Name = outputTableName;

            outputWorkspace.Delete(outputName);

            if (type == OverlayType.Union)
            {
                return gp.Union2(output.RouteEventProperties, true, outputName, trackCancel, "");
            }

            return gp.Intersect2(output.RouteEventProperties, true, outputName, trackCancel, "");
        }

        #endregion
    }
}