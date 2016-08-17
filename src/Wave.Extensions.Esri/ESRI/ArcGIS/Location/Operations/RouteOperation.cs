using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    ///     Provides the ability to perform an operation on event data.
    /// </summary>
    /// <typeparam name="T">The type of event data.</typeparam>
    public abstract class RouteOperation<T> where T : EventData
    {
        #region Public Methods

        /// <summary>
        ///     Executes the operation using the specified event data.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="workspace">The workspace.</param>
        /// <param name="trackCancel">The track cancel.</param>
        /// <returns>Returns a <see cref="ITable" /> representing the results of the operation.</returns>
        public abstract ITable Execute(T eventData, IWorkspace workspace, ITrackCancel trackCancel);

        #endregion
    }
}