using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Documents;

using ESRI.ArcGIS.Geodatabase;

namespace Wave.Searchability.Data
{
    [DataContract(Name = "request")]
    public class SearchableRequest
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRequest" /> class.
        /// </summary>
        public SearchableRequest()
        {
            this.Items = new List<SearchableSet>();
            this.Threshold = 200;
            this.ComparisonOperator = ComparisonOperator.Like;
            this.LogicalOperator = LogicalOperator.Or;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the comparison operator.
        /// </summary>
        /// <value>
        ///     The comparison operator.
        /// </value>
        [DataMember(Name = "comparisonOperator")]
        public ComparisonOperator ComparisonOperator { get; set; }

        /// <summary>
        ///     Gets or sets the keywords.
        /// </summary>
        /// <value>
        ///     The keywords.
        /// </value>
        [DataMember(Name = "keywords")]
        public string Keywords { get; set; }

        /// <summary>
        ///     Gets or sets the logical operator.
        /// </summary>
        /// <value>
        ///     The logical operator.
        /// </value>
        [DataMember(Name = "logicalOperator")]
        public LogicalOperator LogicalOperator { get; set; }

        /// <summary>
        ///     Gets or sets the sets.
        /// </summary>
        /// <value>
        ///     The sets.
        /// </value>
        [DataMember(Name = "items")]
        public IList<SearchableSet> Items { get; set; }

        /// <summary>
        ///     Gets or sets the threshold.
        /// </summary>
        /// <value>
        ///     The threshold.
        /// </value>
        [DataMember(Name = "threshold")]
        public int Threshold { get; set; }

        #endregion
    }
}