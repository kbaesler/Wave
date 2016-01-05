using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using ESRI.ArcGIS.Geodatabase;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{
    internal class MapSearchServiceViewModel : BaseViewModel
    {
        #region Constructors

        public MapSearchServiceViewModel(IEnumerable<SearchableSet> sets)
        {
            this.Sets = new ObservableCollection<SearchableSet>(sets.OrderBy(o => o.Name));
            this.ComparisonOperators = new[] {ComparisonOperator.Like, ComparisonOperator.StartsWith, ComparisonOperator.EndsWith, ComparisonOperator.Equals};
        }

        #endregion

        #region Public Properties

        public ComparisonOperator[] ComparisonOperators { get; set; }
        public ObservableCollection<SearchableSet> Sets { get; set; }
        public string Keywords { get; set; }

        #endregion
    }
}