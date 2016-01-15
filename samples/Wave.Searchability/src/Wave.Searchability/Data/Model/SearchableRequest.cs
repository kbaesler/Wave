using System.Collections.Generic;
using System.Runtime.Serialization;

using ESRI.ArcGIS.Geodatabase;

namespace Wave.Searchability.Data
{
    [DataContract(Name = "request")]
    public abstract class SearchableRequest
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableRequest" /> class.
        /// </summary>
        protected SearchableRequest()
        {
            this.Inventory = new List<SearchableInventory>();
            this.Threshold = 200;
            this.ComparisonOperator = ComparisonOperator.Contains;
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
        /// Gets or sets the inventory.
        /// </summary>
        /// <value>
        /// The inventory.
        /// </value>
        [DataMember(Name = "inventory")]
        public List<SearchableInventory> Inventory { get; set; }

        /// <summary>
        ///     Gets or sets the keyword.
        /// </summary>
        /// <value>
        ///     The keyword.
        /// </value>
        [DataMember(Name = "keyword")]
        public string Keyword { get; set; }

        /// <summary>
        ///     Gets or sets the logical operator.
        /// </summary>
        /// <value>
        ///     The logical operator.
        /// </value>
        [DataMember(Name = "logicalOperator")]
        public LogicalOperator LogicalOperator { get; set; }

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