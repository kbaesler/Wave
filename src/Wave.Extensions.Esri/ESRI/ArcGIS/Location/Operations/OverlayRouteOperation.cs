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

                return sourceSelection.Overlay(eventData.Segmentation, overlaySelection, eventData.OverlaySegmentation, eventData.OverlayType, eventData.Segmentation, eventData.OutputTableName, workspace, trackCancel);
            }

            return eventTable.Overlay(eventData.Segmentation, overlayTable, eventData.OverlaySegmentation, eventData.OverlayType, eventData.Segmentation, eventData.OutputTableName, workspace, trackCancel);
        }

        #endregion
    }
}