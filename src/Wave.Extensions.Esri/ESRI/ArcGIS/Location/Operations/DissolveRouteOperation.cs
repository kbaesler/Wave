using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Removes redundant information from event tables or separates event tables having more than one descriptive
    ///     attribute into individual tables
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteOperation{DissolveRouteEventData}" />
    public class DissolveRouteOperation : RouteOperation<DissolveRouteEventData>
    {
        #region Public Methods

        /// <summary>
        ///     Events will be aggregated wherever there is measure overlap.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="workspace">The workspace that contains the event data.</param>
        /// <param name="trackCancel">The object that allows for monitoring the progress.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        public override ITable Execute(DissolveRouteEventData eventData, IWorkspace workspace, ITrackCancel trackCancel)
        {
            var eventTable = workspace.GetTable("", eventData.EventTableName);

            return eventTable.Dissolve(eventData.Segmentation, workspace, eventData.OutputTableName, eventData.Segmentation, trackCancel, eventData.Fields);
        }

        #endregion
    }
}