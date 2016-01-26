using System.Runtime.InteropServices;
using System.Windows;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.BaseClasses;

using Miner.Framework;

using Wave.Searchability.Events;
using Wave.Searchability.Services;

namespace Wave.Searchability.Extensions
{
    [Guid("DB44276A-8C24-4C4E-A6FF-113198EE9DC9")]
    [ProgId("Wave.Searchability.Extensions.SearchServiceExtension")]
    [ComVisible(true)]
    public class SearchServiceExtension : BaseExtension
    {
        #region Constants

        public const string ExtensionName = "Wave Searchability Extension";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchServiceExtension" /> class.
        /// </summary>
        public SearchServiceExtension()
            : base(ExtensionName)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initialization function for extension
        /// </summary>
        /// <param name="initializationData">ESRI Application Reference</param>
        public override void Startup(ref object initializationData)
        {
            base.Startup(ref initializationData);

            EventAggregator.GetEvent<MapSearchServiceRequestEvent>().Subscribe((request) =>
            {
                var searchService = new MapSearchService();
                var task = searchService.FindAsync(request, Document.ActiveMap);
                task.ContinueWith(t => EventAggregator.GetEvent<SearchableResponseEvent>().Publish(t.Result));
            });
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified CLSID.
        /// </summary>
        /// <param name="CLSID">The CLSID.</param>
        [ComRegisterFunction]
        internal static void Register(string CLSID)
        {
            MxExtension.Register(CLSID);
        }

        /// <summary>
        ///     Unregisters the specified CLSID.
        /// </summary>
        /// <param name="CLSID">The CLSID.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string CLSID)
        {
            MxExtension.Unregister(CLSID);
        }

        #endregion
    }
}