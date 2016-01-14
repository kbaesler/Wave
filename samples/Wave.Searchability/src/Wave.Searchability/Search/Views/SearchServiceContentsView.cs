using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.BaseClasses;

using Miner.Framework;

using Wave.Searchability.Extensions;
using Wave.Searchability.Services;

namespace Wave.Searchability.Views
{
    [Guid("53DF13CF-2EF3-45AD-B887-320F2FBD386A")]
    [ProgId("Wave.Searchability.Views.SearchServiceContentsView")]
    [ComVisible(true)]
    public sealed class SearchServiceContentsView : BaseContentsView
    {
        #region Fields

        private ElementHost _ElementHost;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchServiceContentsView" /> class.
        /// </summary>
        public SearchServiceContentsView()
            : base("Search")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the HWND of the contents view.
        /// </summary>
        public override int hWnd
        {
            get
            {
                if (_ElementHost == null) return 0;
                return _ElementHost.Handle.ToInt32();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Called when the contents view is selected.
        /// </summary>
        /// <param name="parentHWnd">The parent window handle.</param>
        /// <param name="document">The document.</param>
        public override void Activate(int parentHWnd, IMxDocument document)
        {
            if (_ElementHost == null)
            {
                var bootstrapContainer = (IBootstrapContainer) Document.FindExtensionByName(SearchServiceExtension.ExtensionName);                
                var eventAggregator = bootstrapContainer.GetService<IEventAggregator>();
                if (eventAggregator != null)
                {
                    var dataContext = new SearchServiceViewModel(eventAggregator);

                    _ElementHost = new ElementHost();
                    _ElementHost.Child = new SearchServiceView() {DataContext = dataContext};
                    _ElementHost.Dock = DockStyle.Fill;
                }
            }
        }

        /// <summary>
        /// Called when the contents view has been deactivated or when the tab in the table of contents has changed.
        /// </summary>
        public override void Deactivate()
        {
            if (!base.Visible)
            {
                if (_ElementHost != null && _ElementHost.Child != null)
                {
                    var dataContext = ((SearchServiceView) _ElementHost.Child).DataContext as SearchServiceViewModel;
                    if (dataContext != null)
                    {
                        dataContext.Dispose();
                    }

                    _ElementHost = null;
                }
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers class.
        /// </summary>
        /// <param name="CLSID">GUID</param>
        [ComRegisterFunction]
        internal static void Register(string CLSID)
        {
            ContentsViews.Register(CLSID);
        }

        /// <summary>
        ///     Unregisters class.
        /// </summary>
        /// <param name="regKey">GUID</param>
        [ComUnregisterFunction]
        internal static void Unregister(string CLSID)
        {
            ContentsViews.Unregister(CLSID);
        }

        #endregion
    }
}