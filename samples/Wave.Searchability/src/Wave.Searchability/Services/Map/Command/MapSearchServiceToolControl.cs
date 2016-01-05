using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.BaseClasses;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using System.Windows.Forms.Integration;

using Miner.Framework;

using Wave.Searchability.Data;
using Wave.Searchability.Data.Configuration;

namespace Wave.Searchability.Services
{
    public sealed class MapSearchServiceToolControl : BaseMxCommand, IToolControl
    {
        private ElementHost _ElementHost;

        public MapSearchServiceToolControl()
            : base("MapSearchServiceToolControl", "Search", "Wave", "Search", "Search")
        {
            
        }

        public bool OnDrop(esriCmdBarType barType)
        {
            return barType == esriCmdBarType.esriCmdBarTypeToolbar;
        }

        public void OnFocus(ICompletionNotify complete)
        {
            
        }

        public int hWnd
        {
            get { return _ElementHost.Handle.ToInt32(); }
        }

        /// <summary>
        /// Called when the command is created inside the application.
        /// </summary>
        /// <param name="hook">A reference to the application in which the command was created.
        ///                 The hook may be an IApplication reference (for commands created in ArcGIS Desktop applications)
        ///                 or an IHookHelper reference (for commands created on an Engine ToolbarControl).
        ///             </param>
        public override void OnCreate(object hook)
        {
            base.OnCreate(hook);
           
            var eventArregator = Document.FindExtensionByName(MapSearchServiceExtension.ExtensionName);           
            var dataContext = new MapSearchServiceViewModel(null);

            _ElementHost = new ElementHost();
            _ElementHost.Child = new MapSearchServiceView() { DataContext = dataContext};
        }
    }
}
