using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Removes redundant information from event tables or separates event tables having more than one descriptive
    ///     attribute into individual tables
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteOperation{DissolveRouteEventData}" />
    public static class DissolveRouteOperationExtensions
    {
        #region Public Methods

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
        public static ITable Dissolve(this ITable table, IRouteMeasureSegmentation source, IWorkspace outputWorkspace, string outputTableName, IRouteMeasureSegmentation output, ITrackCancel trackCancel, params string[] dissolveFields)
        {
            var outputName = new TableNameClass();
            outputName.WorkspaceName = (IWorkspaceName) ((IDataset) outputWorkspace).FullName;
            outputName.Name = outputTableName;

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