using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteEventData{RouteMeasureSegmentation}" />    
    [DataContract]
    public class DissolveRouteEventData : EventData<RouteMeasureSegmentation>
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