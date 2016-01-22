using System.Runtime.InteropServices;
using System.Windows;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.BaseClasses;

using Miner.Framework;

using Wave.Searchability.Events;
using Wave.Searchability.Services;

namespace Wave.Searchability.Extensions
{
    /// <summary>
    ///     Provides access to the extension container.
    /// </summary>
    public static class ExtensionContainer
    {
        #region Fields

        private static IExtensionContainer _Instance;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IExtensionContainer Instance
        {
            get { return _Instance ?? (_Instance = Document.FindExtensionByName(SearchServiceExtension.ExtensionName) as IExtensionContainer); }
        }

        #endregion
    }

    [Guid("DB44276A-8C24-4C4E-A6FF-113198EE9DC9")]
    [ProgId("Wave.Searchability.Extensions.SearchServiceExtension")]
    [ComVisible(true)]
    public class SearchServiceExtension : BaseExtensionContainer
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
            this.AddService(typeof (IEventAggregator), new EventAggregator());
            this.AddService(typeof (IMapSearchService), new MapSearchService());

            base.Startup(ref initializationData);

            var eventAggregator = this.GetService<IEventAggregator>();
            var searchService = this.GetService<IMapSearchService>();

            eventAggregator.GetEvent<MapSearchServiceRequestEvent>().Subscribe((request) =>
            {
                var task = searchService.FindAsync(request, Document.ActiveMap);
                task.ContinueWith(t => eventAggregator.GetEvent<SearchableResponseEvent>().Publish(t.Result));
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