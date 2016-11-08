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

            return this.Execute(eventTable, eventData.Segmentation, workspace, eventData.OutputTableName, eventData.Segmentation, trackCancel, eventData.Fields);
        }

        /// <summary>
        ///     Events will be aggregated wherever there is measure overlap.
        /// </summary>
        /// <param name="table">The table whose rows will be aggregated.</param>
        /// <param name="source">Parameter consisting of the route location fields and the type of events in the input event table.</param>
        /// <param name="outputWorkspace">The workspace that will contain the dissolved table.</param>
        /// <param name="outputTableName">The table to be created.</param>
        /// <param name="output">
        ///     Parameter consisting of the route location fields and the type of events in the dissolve event
        ///     table.
        /// </param>
        /// <param name="trackCancel">The object that allows for monitoring the progress.</param>
        /// <param name="dissolveFields">The field(s)used to aggregate rows.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the table that has been created.</returns>
        public ITable Execute(ITable table, RouteMeasureSegmentation source, IWorkspace outputWorkspace, string outputTableName, RouteMeasureSegmentation output, ITrackCancel trackCancel, params string[] dissolveFields)
        {
            IDatasetName outputName = outputWorkspace.Define(outputTableName, new TableNameClass());
            outputWorkspace.Delete(outputName);

            IRouteMeasureEventGeoprocessor2 gp = new RouteMeasureGeoprocessorClass();
            gp.InputEventProperties = source.EventProperties;
            gp.InputTable = table;
            gp.KeepZeroLengthLineEvents = false;

            return gp.Dissolve2(output.EventProperties, dissolveFields, outputName, trackCancel, "");
        }

        #endregion
    }
}