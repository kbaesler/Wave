using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using ESRI.ArcGIS.Geodatabase;

using Wave.Searchability.Data;
using Wave.Searchability.Events;
using Wave.Searchability.Services;

namespace Wave.Searchability.Views
{
    internal class SearchServiceViewModel : BaseViewModel
    {
        #region Fields

        private ObservableCollection<SearchableInventory> _Items;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchServiceViewModel" /> class.
        /// </summary>
        public SearchServiceViewModel()
        {
            EventAggregator.GetEvent<SearchableInventoryEvent>().Subscribe(items =>
            {
                this.Items = new ObservableCollection<SearchableInventory>(items);
                this.CurrentItem = this.Items.FirstOrDefault();
            });           

            this.ComparisonOperators = new Dictionary<ComparisonOperator, string>
            {
                {ComparisonOperator.Contains, "Contains"},
                {ComparisonOperator.StartsWith, "Start With"},
                {ComparisonOperator.EndsWith, "Ends With"},
                {ComparisonOperator.Equals, "Equals"}
            };

            this.ComparisonOperator = this.ComparisonOperators.First().Key;

            this.Extents = new Dictionary<MapSearchServiceExtent, string>
            {
                {MapSearchServiceExtent.WithinAnyExtent, "Within Any Extent"},
                {MapSearchServiceExtent.WithinCurrentExtent, "Within Current Extent"},
                {MapSearchServiceExtent.WithinOrOverlappingCurrentExtent, "Within Or Overlapping Current Extent"}
            };

            this.Extent = this.Extents.Skip(1).First().Key;

            this.SearchCommand = new DelegateCommand(o => EventAggregator.GetEvent<MapSearchServiceRequestEvent>().Publish(new MapSearchServiceRequest()
            {
                Inventory = new List<SearchableInventory>(new[] {this.CurrentItem}),
                ComparisonOperator = this.ComparisonOperator,
                Extent = this.Extent,
                Keyword = this.Keyword
            }));
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        ///     Gets or sets the cancel command.
        /// </summary>
        /// <value>
        ///     The cancel command.
        /// </value>
        public DelegateCommand CancelCommand { get; set; }

        /// <summary>
        ///     Gets or sets the comparison operator.
        /// </summary>
        /// <value>
        ///     The comparison operator.
        /// </value>
        public ComparisonOperator ComparisonOperator { get; set; }

        /// <summary>
        ///     Gets or sets the comparison operators.
        /// </summary>
        /// <value>
        ///     The comparison operators.
        /// </value>
        public Dictionary<ComparisonOperator, string> ComparisonOperators { get; set; }

        /// <summary>
        ///     Gets or sets the current item.
        /// </summary>
        /// <value>
        ///     The current item.
        /// </value>
        public SearchableInventory CurrentItem { get; set; }

        /// <summary>
        ///     Gets or sets the extent.
        /// </summary>
        /// <value>
        ///     The extent.
        /// </value>
        public MapSearchServiceExtent Extent { get; set; }

        /// <summary>
        ///     Gets or sets the extents.
        /// </summary>
        /// <value>
        ///     The extents.
        /// </value>
        public Dictionary<MapSearchServiceExtent, string> Extents { get; set; }

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public ObservableCollection<SearchableInventory> Items
        {
            get { return _Items; }
            set
            {
                base.OnPropertyChanging("Items");

                _Items = value;

                base.OnPropertyChanged("Items");
            }
        }

        /// <summary>
        ///     Gets or sets the keyword.
        /// </summary>
        /// <value>
        ///     The keyword.
        /// </value>
        public string Keyword { get; set; }

        /// <summary>
        ///     Gets or sets the search command.
        /// </summary>
        /// <value>
        ///     The search command.
        /// </value>
        public DelegateCommand SearchCommand { get; set; }

        #endregion
    }
}