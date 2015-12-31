using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace Wave.Searchability.Data.Configuration
{
    public class SearchServiceConfiguration
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchServiceConfiguration" /> class.
        /// </summary>
        public SearchServiceConfiguration()
        {
            this.Threshold = 200;
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
        ///     Gets or sets the keywords.
        /// </summary>
        /// <value>
        ///     The keywords.
        /// </value>
        public string Keywords { get; set; }

        /// <summary>
        ///     Gets or sets the logical operator.
        /// </summary>
        /// <value>
        ///     The logical operator.
        /// </value>
        public LogicalOperator LogicalOperator { get; set; }

        /// <summary>
        ///     Gets or sets the sets.
        /// </summary>
        /// <value>
        ///     The sets.
        /// </value>
        public List<SearchableSet> Sets { get; set; }

        /// <summary>
        ///     Gets or sets the threshold.
        /// </summary>
        /// <value>
        ///     The threshold.
        /// </value>
        public int Threshold { get; set; }

        #endregion
    }
}