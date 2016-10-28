using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.EventData{RouteMeasureSegmentation}" />
    [DataContract]
    public class ConcatenateRouteEventData : EventData<RouteMeasureSegmentation>
    {
        #region Public Properties

        /// <summary>
        ///     Gets the fields that will be dissolved.
        /// </summary>
        [DataMember]
        public string[] Fields { get; set; }

        #endregion
    }
}