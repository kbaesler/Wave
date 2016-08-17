using System.Runtime.Serialization;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// </summary>
    /// <seealso cref="ESRI.ArcGIS.Location.RouteEventData" />
    [DataContract]
    public class DissolveRouteEventData : RouteEventData
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