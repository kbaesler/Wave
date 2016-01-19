using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Windows.Forms.VisualStyles;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{
    /// <summary>
    ///     A service contract for searching the active map session for table(s), class(es) and relationship(s).
    /// </summary>
    /// <typeparam name="TSearchableRequest">The type of the searchable request.</typeparam>
    /// <typeparam name="TDataSource">The type of the data source.</typeparam>
    [ServiceContract]
    public interface ISearchableService<in TSearchableRequest, in TDataSource> where TSearchableRequest : SearchableRequest
    {
        #region Public Methods

        /// <summary>
        ///     Searches the data source using the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the results.
        /// </returns>
        SearchableResponse Find(TSearchableRequest request, TDataSource source);


        /// <summary>
        ///     Searches the active data source using the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the results.
        /// </returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Find", Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        SearchableResponse Find(TSearchableRequest request);

        #endregion
    }

    /// <summary>
    ///     A search service that allows for 'google-like' search capabilities
    ///     in the scense that given a keyword it will search all of the approriate fields in the table or feature classes
    ///     specified in the configurations.
    /// </summary>
    /// <typeparam name="TSearchableRequest">The type of the searchable request.</typeparam>
    /// <typeparam name="TDataSource">The type of the data source.</typeparam>
    public abstract class SearchableService<TSearchableRequest, TDataSource> : ISearchableService<TSearchableRequest, TDataSource>
        where TSearchableRequest : SearchableRequest, new()
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableService{TSearchableRequest, TDataSource}" /> class.
        /// </summary>
        protected SearchableService()
        {
            this.ConcurrentDictionary = new ConcurrentDictionary<string, ConcurrentBag<int>>();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the action that will cancel the operation when the threshold has been reached.
        /// </summary>
        protected Action CancelIfAtThreshold { get; set; }

        /// <summary>
        ///     Gets or sets the concurrent dictionary.
        /// </summary>
        /// <value>
        ///     The concurrent dictionary.
        /// </value>
        protected ConcurrentDictionary<string, ConcurrentBag<int>> ConcurrentDictionary { get; set; }

        #endregion

        #region ISearchableService<TSearchableRequest,TDataSource> Members

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public SearchableResponse Find(TSearchableRequest request)
        {
            using (var cts = new CancellationTokenSource())
            {
                this.SetCancelIfAtThreshold(cts, request);

                try
                {
                    this.ConcurrentDictionary.Clear();
                    this.Find(request, cts.Token);
                }                
                catch (AggregateException e)
                {
                    cts.Cancel();

                    Log.Error(this, e.Flatten());
                }
            }

            return new SearchableResponse(this.ConcurrentDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()));
        }

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="source">The map.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public virtual SearchableResponse Find(TSearchableRequest request, TDataSource source)
        {
            using (var cts = new CancellationTokenSource())
            {
                this.SetCancelIfAtThreshold(cts, request);

                try
                {
                    this.ConcurrentDictionary.Clear();
                    this.Find(request, source, cts.Token);
                }              
                catch (AggregateException e)
                {
                    cts.Cancel();

                    Log.Error(this, e.Flatten());
                }
            }

            return new SearchableResponse(this.ConcurrentDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()));
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the specified row (or feature) to the response.
        /// </summary>
        /// <param name="row">The row or feature.</param>
        /// <param name="layer">The feature layer for the row (when the row is a feature class).</param>
        /// <param name="isFeatureClass">if set to <c>true</c> when the row is a feature class.</param>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        protected virtual void Add(IRow row, IFeatureLayer layer, bool isFeatureClass, TSearchableRequest request, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var name = (layer == null) ? ((IDataset) row.Table).Name : layer.Name;
            this.ConcurrentDictionary.AddOrUpdate(name, s =>
            {
                var bag = new ConcurrentBag<int>();
                bag.Add(row.OID);

                return bag;
            }, (s, bag) =>
            {
                if (!bag.Contains(row.OID))
                    bag.Add(row.OID);

                return bag;
            });

            this.CancelIfAtThreshold();
        }

        /// <summary>
        ///     Compiles the query expression for the class using the request.
        /// </summary>
        /// <param name="searchClass">The build class.</param>
        /// <param name="request">The request.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     A <see cref="string" /> for the corresponding fields and values; otherwise null when no fields are present.
        /// </returns>
        protected virtual string CompileExpression(ITable searchClass, TSearchableRequest request, SearchableItem item)
        {
            if (item.Fields.Any(f => f.Name.Equals(Searchable.Any)))
            {
                return searchClass.CreateExpression(request.Keyword, request.ComparisonOperator, request.LogicalOperator);
            }

            StringBuilder whereClause = new StringBuilder();
            bool tagOpen = false;

            // We need to keep the OR values grouped so the results are correct.
            foreach (SearchableField sf in item.Fields.OrderBy(f => !f.Visible && !string.IsNullOrEmpty(f.Value)))
            {
                // Ensure that the field exists.
                int index = searchClass.FindField(sf.Name);
                if (index == -1)
                    throw new IndexOutOfRangeException(string.Format("The '{0}' doesn't have a {1} field.", ((IDataset) searchClass).Name, sf.Name));

                IField field = searchClass.Fields.Field[index];
                string value = null;
                LogicalOperator logicalOperator;

                if (sf.Visible)
                {
                    // When visible to the user we need to use the value they entered.
                    logicalOperator = LogicalOperator.And;
                    value = sf.Value;
                }
                else if (!sf.Visible && !string.IsNullOrEmpty(sf.Value))
                {
                    // When a default value is specified but not shown to the user use that value.
                    logicalOperator = LogicalOperator.And;
                    value = sf.Value;
                }
                else
                {
                    // Use the keyword.
                    value = request.Keyword;
                    logicalOperator = LogicalOperator.Or;
                }

                // End the parentheses.
                if (tagOpen && logicalOperator == LogicalOperator.And)
                {
                    whereClause.Append(")"); // Add the closing parentheses
                    tagOpen = false;
                }

                // Create the WHERE clause.
                string expression = searchClass.CreateExpression(value, request.ComparisonOperator, logicalOperator, field);
                if (!string.IsNullOrEmpty(expression))
                {
                    if (whereClause.Length > 0)
                    {
                        // Append the OR operator.
                        whereClause.Append(string.Format(" {0} ", logicalOperator));
                    }
                    else if (!tagOpen && whereClause.Length == 0 && logicalOperator == LogicalOperator.Or)
                    {
                        // Avoid unecessary parentheses.
                        if (item.Fields.Count > 1)
                        {
                            tagOpen = true;
                            whereClause.Append("("); // Add the opening parentheses.
                        }
                    }

                    // Append to the end of the WHERE clause.
                    whereClause.Append(expression);
                }
            }

            // End the parentheses.
            if (tagOpen)
            {
                whereClause.Append(")"); // Add the closing parentheses
            }

            return whereClause.ToString();
        }

        protected abstract void Find(TSearchableRequest request, TDataSource source, CancellationToken token);
        protected abstract void Find(TSearchableRequest request, CancellationToken token);        

        #endregion

        #region Private Methods

        /// <summary>
        ///     Sets the cancellation token source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="request">The request.</param>
        private void SetCancelIfAtThreshold(CancellationTokenSource source, TSearchableRequest request)
        {
            this.CancelIfAtThreshold = () =>
            {
                if (request.Threshold > 0)
                {
                    int constraints = 0;

                    switch (request.ThresholdConstraint)
                    {
                        case ThresholdConstraints.Request:

                            if (this.ConcurrentDictionary.Values.Sum(o => o.Count) >= request.Threshold)
                                source.Cancel();

                            break;

                        case ThresholdConstraints.Inventory:

                            foreach (var i in request.Inventory)
                            {
                                var inventory = i;
                                var items = this.ConcurrentDictionary.Where(o => inventory.Items.Any(item => item.Name.Equals(o.Key))).ToList();
                                if (items.Count == inventory.Items.Count)
                                {
                                    if (items.Sum(item => item.Value.Count) >= request.Threshold)
                                    {
                                        constraints++;
                                        break;
                                    }
                                }
                            }

                            if (constraints == request.Inventory.Count)
                                source.Cancel();

                            break;

                        case ThresholdConstraints.Item:

                            foreach (var i in request.Inventory)
                            {
                                var inventory = i;
                                var items = this.ConcurrentDictionary.Where(o => inventory.Items.Any(item => item.Name.Equals(o.Key)));
                                if (items.All(item => item.Value.Count >= request.Threshold))
                                    constraints++;

                            }

                            if (constraints == request.Inventory.Sum(i => i.Items.Count))
                                source.Cancel();

                            break;
                    }
                }
            };
        }

        #endregion
    }
}