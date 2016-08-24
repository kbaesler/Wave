using System;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Removes redundant information from event tables or separates event tables having more than one descriptive
    ///     attribute into individual tables
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteOperation{ConcatenateRouteEventData}" />
    public class ConcatenateRouteOperation : RouteOperation<ConcatenateRouteEventData>
    {
        #region Public Methods

        /// <summary>
        ///     Events will be aggregated where the to-measure of one event matches the from-measure of the next event. This option
        ///     is applicable only for line events.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="workspace">The workspace that contains the event data.</param>
        /// <param name="trackCancel">The object that allows for monitoring the progress.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        public override ITable Execute(ConcatenateRouteEventData eventData, IWorkspace workspace, ITrackCancel trackCancel)
        {
            var eventTable = workspace.GetTable("", eventData.EventTableName);
            var linear = eventData.Segmentation as IRouteMeasureLineSegmentation;
            if (linear == null) throw new ArgumentException("This operation is applicable only for line events.");

            return eventTable.Concatenate(linear, workspace, eventData.OutputTableName, eventData.Segmentation, trackCancel, eventData.Fields);
        }

        #endregion
    }
}