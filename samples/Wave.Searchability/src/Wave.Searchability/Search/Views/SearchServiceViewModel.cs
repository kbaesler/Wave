using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using ESRI.ArcGIS.Geodatabase;

using Wave.Searchability.Data;
using Wave.Searchability.Services;

namespace Wave.Searchability.Views
{
    internal class SearchServiceViewModel : BaseViewModel
    {
        #region Fields

        private readonly IEventAggregator _EventAggregator;
        private readonly SubscriptionToken _SubscriptionToken;
        private ObservableCollection<SearchableSet> _Items;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchServiceViewModel" /> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SearchServiceViewModel(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
            _SubscriptionToken = eventAggregator.GetEvent<CompositePresentationEvent<IEnumerable<SearchableSet>>>().Subscribe(sets =>
            {
                this.Items = new ObservableCollection<SearchableSet>(sets);
                this.CurrentItem = this.Items.Select(o => o.Items.FirstOrDefault()).FirstOrDefault();
            });

            this.ComparisonOperators = new Dictionary<ComparisonOperator, string>
            {
                {ComparisonOperator.Contains, "Contains"},
                {ComparisonOperator.StartsWith, "Start With"},
                {ComparisonOperator.EndsWith, "Ends With"},
                {ComparisonOperator.Equals, "Equals"}
            };

            this.Extents = new Dictionary<MapSearchServiceExtent, string>
            {
                {MapSearchServiceExtent.WithinAnyExtent, "Any"},
                {MapSearchServiceExtent.WithinCurrentExtent, "Current"},
                {MapSearchServiceExtent.WithinCurrentOrOverlappingExtent, "Current or Overlaping"},
            };

            this.SearchCommand = new DelegateCommand((o) => eventAggregator.GetEvent<CompositePresentationEvent<MapSearchServiceRequest>>().Publish(new MapSearchServiceRequest()
            {
                Items = new List<SearchableItem>(new[] {this.CurrentItem}),
                ComparisonOperator = this.ComparisonOperator,
                Extent = this.Extent,
                Keyword = this.Keyword,
                LogicalOperator = LogicalOperator.Or,
                Threshold = 200
            }));
        }

        #endregion

        #region Public Properties

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
        public SearchableItem CurrentItem { get; set; }

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
        public ObservableCollection<SearchableSet> Items
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

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _EventAggregator.GetEvent<CompositePresentationEvent<IEnumerable<SearchableSet>>>().Unsubscribe(_SubscriptionToken);
        }

        #endregion
    }
}