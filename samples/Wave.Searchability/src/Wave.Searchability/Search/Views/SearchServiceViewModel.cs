using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using ESRI.ArcGIS.Geodatabase;

using Miner.Framework;

using Wave.Searchability.Data;
using Wave.Searchability.Services;

namespace Wave.Searchability.Views
{
    internal class SearchableSetEvent : CompositePresentationEvent<IEnumerable<SearchableSet>>
    {
    }

    internal class SearchServiceViewModel : BaseViewModel
    {
        #region Fields

        private readonly IEventAggregator _EventAggregator;
        private readonly SubscriptionToken _SubscriptionToken;
        private ObservableCollection<SearchableSet> _Items;

        #endregion

        #region Constructors

        public SearchServiceViewModel(IEventAggregator eventAggregator, IMapSearchService searchService)
        {
            _EventAggregator = eventAggregator;
            _SubscriptionToken = eventAggregator.GetEvent<SearchableSetEvent>().Subscribe(sets =>
            {
                this.Items = new ObservableCollection<SearchableSet>(sets);
                this.CurrentItem = this.Items.Select(o => o.Items.FirstOrDefault()).FirstOrDefault();
            }, ThreadOption.BackgroundThread);

            this.ComparisonOperators = new Dictionary<ComparisonOperator, string>
            {
                {ComparisonOperator.Contains, "Contains"},
                {ComparisonOperator.StartsWith, "Start With"},
                {ComparisonOperator.EndsWith, "Ends With"},
                {ComparisonOperator.Equals, "Equals"}
            };

            this.Extents = new Dictionary<MapSearchServiceExtent, string>
            {
                {MapSearchServiceExtent.Any, "Any"},
                {MapSearchServiceExtent.WithinCurrent, "Current"},
                {MapSearchServiceExtent.WithinCurrentOrOverlapping, "Current or Overlaping"},
            };

            this.SearchCommand = new DelegateCommand((o) => this.Find(searchService, eventAggregator));
        }

        #endregion

        #region Public Properties

        public ComparisonOperator ComparisonOperator { get; set; }
        public Dictionary<ComparisonOperator, string> ComparisonOperators { get; set; }

        public SearchableItem CurrentItem { get; set; }
        public MapSearchServiceExtent Extent { get; set; }
        public Dictionary<MapSearchServiceExtent, string> Extents { get; set; }

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

        public string Keywords { get; set; }
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

            _EventAggregator.GetEvent<SearchableSetEvent>().Unsubscribe(_SubscriptionToken);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Finds the specified search service.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        private void Find(IMapSearchService searchService, IEventAggregator eventAggregator)
        {
            var request = new MapSearchServiceRequest()
            {
                Items = new List<SearchableItem>(new[] {this.CurrentItem}),
                ComparisonOperator = this.ComparisonOperator,
                Extent = this.Extent,
                Keywords = this.Keywords,
                LogicalOperator = LogicalOperator.Or,
                Threshold = 200
            };

            var response = searchService.Find(request, Document.ActiveMap);
            eventAggregator.GetEvent<CompositePresentationEvent<SearchableResponse>>().Publish(response);
        }

        #endregion
    }
}