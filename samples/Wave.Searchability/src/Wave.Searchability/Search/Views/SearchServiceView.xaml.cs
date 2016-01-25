using System.Linq;
using System.Windows;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Controls;
using Miner.Framework;
using Miner.FrameworkUI.Search;
using Miner.Interop;

using Wave.Searchability.Data;
using Wave.Searchability.Events;
using Wave.Searchability.Extensions;

using UserControl = System.Windows.Controls.UserControl;

namespace Wave.Searchability.Views
{
    /// <summary>
    ///     Interaction logic for SearchServiceView.xaml
    /// </summary>
    public partial class SearchServiceView : UserControl
    {
        #region Constructors

        public SearchServiceView()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds the layers.
        /// </summary>
        /// <param name="response">The response.</param>
        private void AddLayers(SearchableResponse response)
        {
            var layers = response.ToSearchResults(Document.ActiveMap.GetLayers<IFeatureLayer>(l => l.Valid).ToList());

            var processor = new StandardResultsProcessor();
            ID8List list = processor.AddResults(layers, mmSearchOptionFlags.mmSOFNone, null);

            IItemNode itemNode = new ListItemNode();
            itemNode.Init(list);

            this.MinerTreeView.InitializeTree(itemNode);
        }

        /// <summary>
        ///     Adds the tables.
        /// </summary>
        /// <param name="response">The response.</param>
        private void AddTables(SearchableResponse response)
        {
            var tables = response.ToSearchResults(Document.ActiveMap.GetTables().ToList());

            var processor = new StandardResultsProcessor();
            ID8List list = processor.AddResults(tables, mmSearchOptionFlags.mmSOFNone, null);

            IItemNode itemNode = new ListItemNode();
            itemNode.Init(list);

            this.MinerTreeView.InitializeTree(itemNode);
        }

        /// <summary>
        ///     Automatics the select first feature.
        /// </summary>
        private void AutoSelectFirst()
        {
            IMMRegistry registry = new MMRegistryClass();
            registry.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, @"Attribute Editor\Selection Tab");
            int autoSelect = (int) registry.Read("AutoSelectFirstFeature", 0);

            if (this.MinerTreeView.TopNode != null)
            {
                // 0 = None
                // 1 = When 1 feature is found.
                // 2 = Always
                if ((autoSelect == 1) || autoSelect == 2)
                {
                    TreeNode firstNode = this.MinerTreeView.TopNode.FirstNode;
                    if (firstNode != null)
                    {
                        this.MinerTreeView.SelectedNode = firstNode;
                        firstNode.Expand();
                    }
                }

                for (int i = 1; i < this.MinerTreeView.Nodes.Count; i++)
                {
                    this.MinerTreeView.Nodes.RemoveAt(i);
                }
            }
        }

        /// <summary>
        ///     Handles the OnSelectionChanged event of the MinerTreeView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionEventArgs" /> instance containing the event data.</param>
        private void MinerTreeView_OnSelectionChanged(object sender, SelectionEventArgs e)
        {
            if (e.Selection != null)
            {
                e.Selection.Reset();
                ID8GeoAssoc item = e.Selection.Next as ID8GeoAssoc;

                if (item != null)
                {
                    IRow row = item.AssociatedGeoRow;
                    var table = row.Table as IRelQueryTable;
                    if (table != null)
                    {
                        row = table.SourceTable.GetRow(row.OID);
                    }

                    this.Editor.ViewRow(row, false);
                }
                else
                {
                    this.Editor.ViewByFieldManager(null, mmDisplayMode.mmdmObject, false);
                }
            }
            else
            {
                this.Editor.ViewByFieldManager(null, mmDisplayMode.mmdmObject, false);
            }
        }

        /// <summary>
        ///     Handles the OnLoaded event of the SearchServiceView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SearchServiceView_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.MinerTreeView.ContextCategory = D8SelectionTreeTool.CatID;

            var eventAggregator = ExtensionContainer.Instance.GetService<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SearchableResponseEvent>().Subscribe(response =>
                {
                    this.MinerTreeView.ClearNodes();

                    this.AddLayers(response);

                    this.AddTables(response);

                    this.AutoSelectFirst();
                }, ThreadOption.UIThread);

                eventAggregator.GetEvent<MapSearchServiceRequestEvent>().Subscribe(request =>
                {
                    this.MinerTreeView.ClearNodes();
                    this.Editor.ViewByFieldManager(null, mmDisplayMode.mmdmObject, false);
                }, ThreadOption.UIThread);
            }
        }

        #endregion
    }
}