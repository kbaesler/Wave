using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ESRI.ArcGIS.BaseClasses;

using Miner.Controls;
using Miner.Framework;
using Miner.FrameworkUI.Search;
using Miner.Interop;

using Wave.Searchability.Data;
using Wave.Searchability.Extensions;

namespace Wave.Searchability.Views
{
    /// <summary>
    /// Interaction logic for SearchServiceView.xaml
    /// </summary>
    public partial class SearchServiceView : UserControl
    {
        public SearchServiceView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the OnDataContextChanged event of the SearchServiceView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SearchServiceView_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var eventAggregator = ExtensionContainer.Instance.GetService<IEventAggregator>();
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<CompositePresentationEvent<SearchableResponse>>().Subscribe((response) =>
                {
                    var results = response.ToSearchResults(Document.ActiveMap);
                    var processor = new StandardResultsProcessor();

                    ID8List list = processor.AddResults(results, mmSearchOptionFlags.mmSOFNone, null);
                    IItemNode itemNode = ListNodeLoader.Load(null, list);
                    this.MinerTreeView.InitializeTree(itemNode);
                });
            }
        }
    }
}
