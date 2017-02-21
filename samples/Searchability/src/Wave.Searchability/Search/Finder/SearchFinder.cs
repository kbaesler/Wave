using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

using Wave.Searchability.Data;
using Wave.Searchability.Search.Views;
using Wave.Searchability.Services;

namespace Wave.Searchability.Search.Finder
{
    [ComVisible(true)]
    [ProgId("Wave.Searchability.SearchFinder")]
    [Guid("136BAF22-4971-4092-9FB2-C4F3847192F9")]
    public class SearchFinder : BaseFinder
    {
        #region Fields

        private readonly CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();
        private readonly SearchFinderControl _Control = new SearchFinderControl();        

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchFinder" /> class.
        /// </summary>
        public SearchFinder()
            : base("Search")
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Number of columns to display in list box.
        /// </summary>
        public override int ColumnCount
        {
            get { return 2; }
        }

        /// <summary>
        ///     UID of menu to popup in list box.
        /// </summary>
        public override UID MenuUID
        {
            get
            {
                IFinder f = new FindFeaturesClass();
                return f.MenuUID;
            }
        }

        /// <summary>
        /// </summary>
        public override int hWnd
        {
            get
            {
                if (_Control == null) return 0;

                return _Control.Handle.ToInt32();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     New search. Clear control input boxes.
        /// </summary>
        public override void NewSearch()
        {
            _Control.Clear();

            var map = base.Application.GetActiveMap();
            var result = this.GetSearchableItems(map);

            EventAggregator.GetEvent<SearchableItemsEvent>().Publish(result);
        }

        /// <summary>
        ///     User requested find to stop.
        /// </summary>
        public override void Stop()
        {
            _CancellationTokenSource.Cancel();
        }

        /// <summary>
        ///     Called whenever ArcMap status changes.
        /// </summary>
        public override void UpdateControl()
        {
        }


        /// <summary>
        /// The column name.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public override string get_ColumnName(int column)
        {
            switch (column)
            {
                case 0:
                    return "OID";

                case 1:
                    return "Layer";

                default:
                    return "";
            }
        }

        /// <summary>
        ///     The column width in Dialog Units (1/4 of avg. char width).
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public override int get_ColumnWidth(int column)
        {
            switch (column)
            {
                case 0:
                    return 40;
                case 1:
                    return 175;
                default:
                    return 0;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Initializes the control.
        /// </summary>
        protected override void InitializeControl()
        {
           
        }

        /// <summary>
        ///     Perform find functionality.
        /// </summary>
        /// <param name="findCallBack">The find call back.</param>
        protected override void InternalFind(IFindCallBack findCallBack)
        {
            try
            {                
                var doc = findCallBack.Application.Document as IMxDocument;
                if (doc == null) return;

                var map = doc.FocusMap;
                if (map == null) return;

                var request = _Control.GetSearchRequest();
                if (request == null) return;

                var service = new MapSearchService();
                var response = service.Find(request, map, _CancellationTokenSource);
                var set = new SetClass();

                foreach (var values in response.Values)
                {
                    for (int i = values.Count - 1; i >= 0; i--)
                    {                        
                        var featureFindData = values[i];
                        set.Add(featureFindData);

                        findCallBack.ColumnValue[0] = featureFindData.Feature.OID.ToString(CultureInfo.InvariantCulture);
                        findCallBack.ColumnValue[1] = featureFindData.Layer.Name;
                        findCallBack.Object = featureFindData;
                        findCallBack.AddNewRow();
                
                        bool quitProcessing;
                        findCallBack.ProcessMessages(out quitProcessing);

                        if (quitProcessing)
                            break;
                    }
                }

                doc.ContextItem = set;
            }
            catch (Exception ex)
            {
                Log.Error(this, ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the searchable items based on the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{Searchable}" /> representing the searchable items in the map.
        /// </returns>
        private IEnumerable<Searchable> GetLayers(IMap map)
        {
            var items = new List<Searchable>();

            foreach (var node in map.GetHierarchy())
            {
                SearchablePackage package = null;
                IEnumerable<IHierarchy<ILayer>> children;

                if (node.Children.Any())
                {
                    package = new SearchablePackage(node.Value.Name);
                    items.Add(package);

                    children = node.Children;
                }
                else
                {
                    children = new[] {node};
                }

                foreach (var child in children)
                {
                    var annotationLayer = child.Value as IAnnotationSublayer;
                    if (annotationLayer != null) continue;

                    var featureLayer = child.Value as IFeatureLayer;
                    if (featureLayer == null) continue;
                    
                    var item = featureLayer.AsSearchableItem();

                    if (package != null)
                        package.Items.Add(item);
                    else
                        items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        ///     Gets the searchable items.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{Searchable}"/> representing the items.</returns>
        private IEnumerable<Searchable> GetSearchableItems(IMap map)
        {
            var items = this.GetInventoryItems(map, SearchabilityInventory.Default);

            var visible = this.GetVisibleLayers(map);
            items.Add(visible);

            var selectable = this.GetSelectableLayers(map);
            items.Add(selectable);

            var layers = this.GetLayers(map);
            items.AddRange(layers);

            var tables = this.GetTables(map);
            items.AddRange(tables);

            return items;
        }

        /// <summary>
        /// Gets the inventory items.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="inventory">The inventory.</param>
        /// <returns></returns>
        private List<Searchable> GetInventoryItems(IMap map, SearchabilityInventory inventory)
        {
            var layers = map.GetLayers<IFeatureLayer>(layer => layer.Valid).ToLookup(kvp => kvp.Name, kvp => kvp.FeatureClass.AliasName);

            for (int i = inventory.Packages.Count - 1; i >= 0; i--)
            {
                var pkg = inventory.Packages[i];
                if (pkg.Items.Any(item => !layers.Contains(item.Name) && !layers.Contains(item.AliasName)))
                {
                    inventory.Packages.RemoveAt(i);
                }
            }

            return inventory.Packages.Cast<Searchable>().ToList();
        }
       
        /// <summary>
        ///     Gets the selectable layers.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="SearchablePackage"/> representing the package.</returns>
        private SearchablePackage GetSelectableLayers(IMap map)
        {
            SearchablePackage package = new SearchablePackage("<Selectable Layers>");

            foreach (var layer in map.GetSelectableLayers())
            {
                var item = layer.AsSearchableItem();
                package.Items.Add(item);
            }

            return package;
        }

        /// <summary>
        ///     Gets the tables.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{Searchable}"/> representing the tables.</returns>
        private IEnumerable<Searchable> GetTables(IMap map)
        {
            return map.GetTables().Select(table => table.AsSearchableItem()).Cast<Searchable>().ToList();
        }

        /// <summary>
        ///     Gets the visible layers.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="SearchablePackage"/> representing the package.</returns>
        private SearchablePackage GetVisibleLayers(IMap map)
        {
            SearchablePackage package = new SearchablePackage("<Visible Layers>");

            foreach (var layer in map.GetVisibleLayers())
            {
                var item = layer.AsSearchableItem();
                package.Items.Add(item);
            }

            return package;
        }

        #endregion
    }
}