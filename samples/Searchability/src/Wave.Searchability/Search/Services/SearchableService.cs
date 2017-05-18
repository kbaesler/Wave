using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        ///     Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Find", Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        SearchableResponse Find(TSearchableRequest request, TDataSource data);

        /// <summary>
        ///     Searches the data source for results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        Task<SearchableResponse> FindAsync(TSearchableRequest request, TDataSource data);

        /// <summary>
        ///     Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "Find", Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Task<SearchableResponse> FindAsync(TSearchableRequest request);

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
            this.ConcurrentDictionary = new ConcurrentDictionary<string, ConcurrentBag<IFeatureFindData2>>();
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
        protected ConcurrentDictionary<string, ConcurrentBag<IFeatureFindData2>> ConcurrentDictionary { get; set; }

        #endregion

        #region ISearchableService<TSearchableRequest,TDataSource> Members

        /// <summary>
        ///     Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public Task<SearchableResponse> FindAsync(TSearchableRequest request)
        {
            return Task.Factory.StartNew(() => this.Find(request));
        }

        /// <summary>
        ///     Searches the data source for results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public Task<SearchableResponse> FindAsync(TSearchableRequest request, TDataSource data)
        {
            return Task.Factory.StartNew(() => this.Find(request, data));
        }

        /// <summary>
        ///     Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public SearchableResponse Find(TSearchableRequest request, TDataSource data)
        {
            using (var source = new CancellationTokenSource())
            {
                return this.Find(request, data, source);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationSource">The cancellation token source.</param>
        /// <returns>
        /// Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public SearchableResponse Find(TSearchableRequest request, TDataSource data, CancellationTokenSource cancellationSource)
        {
            try
            {
                this.ConcurrentDictionary.Clear();
                this.SetCancellationAction(cancellationSource, request);
                this.Find(request, data, cancellationSource.Token);
            }
            catch (Exception e)
            {
                if (!cancellationSource.IsCancellationRequested)
                    cancellationSource.Cancel();

                Log.Error(this, e);
            }

            return new SearchableResponse(this.ConcurrentDictionary);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the specified feature to the response.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="layer">The feature layer for the feature.</param>
        /// <param name="request">The request.</param>
        /// <param name="map">The map.</param>
        /// <param name="token">The token.</param>
        protected virtual void Add(IFeature feature, IFeatureLayer layer, TSearchableRequest request, IMap map, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var data = new FeatureFindDataClass();
            data.Feature = feature;
            data.Layer = layer;
            data.FID = feature.OID;
            data.LayerName = layer.Name;
            data.IFindObject_LayerName = layer.Name;
            data.IFindObject_Value = request.Keyword;
            data.Value = request.Keyword;

            var name = layer.Name;
            if (this.ViolateThresholdContraint(name, request))
                return;

            this.ConcurrentDictionary.AddOrUpdate(name, s =>
            {
                var bag = new ConcurrentBag<IFeatureFindData2>();
                bag.Add(data);

                return bag;
            }, (s, bag) =>
            {
                if (!bag.Contains(data))
                    bag.Add(data);

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
            if (item.Fields.Any(f => f.Equals(SearchableField.Any)))
            {
                return searchClass.CreateExpression(request.Keyword, request.ComparisonOperator, request.LogicalOperator);
            }

            var fields = item.Fields.OrderBy(f => !f.Visible && !string.IsNullOrEmpty(f.Value));
            return this.CompileExpression(searchClass, request, fields.ToArray());
        }

        /// <summary>
        ///     Finds the requested objects using the specified data source.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The source.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        protected abstract void Find(TSearchableRequest request, TDataSource data, CancellationToken token);

        /// <summary>
        ///     Finds the requested objects using the specified data source.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The cancellation token.</param>
        protected abstract void Find(TSearchableRequest request, CancellationToken token);

        #endregion

        #region Private Methods

        /// <summary>
        ///     Compiles the query expression for the class using the request.
        /// </summary>
        /// <param name="searchClass">The build class.</param>
        /// <param name="request">The request.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>
        ///     A <see cref="string" /> for the corresponding fields and values; otherwise null when no fields are present.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        private string CompileExpression(ITable searchClass, TSearchableRequest request, params SearchableField[] fields)
        {
            StringBuilder whereClause = new StringBuilder();
            bool tagOpen = false;

            foreach (var f in fields)
            {
                // Ensure that the field exists.
                int index = searchClass.FindField(f.Name);
                if (index == -1)
                    throw new IndexOutOfRangeException(string.Format("The '{0}' doesn't have a {1} field.", ((IDataset) searchClass).Name, f.Name));

                IField field = searchClass.Fields.Field[index];
                string value;
                LogicalOperator logicalOperator;

                if (f.Visible)
                {
                    // When visible to the user we need to use the value they entered.
                    logicalOperator = LogicalOperator.And;
                    value = f.Value;
                }
                else if (!f.Visible && !string.IsNullOrEmpty(f.Value))
                {
                    // When a default value is specified but not shown to the user use that value.
                    logicalOperator = LogicalOperator.And;
                    value = f.Value;
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
                        if (fields.Length > 1)
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

        /// <summary>
        ///     Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        private SearchableResponse Find(TSearchableRequest request)
        {
            using (var source = new CancellationTokenSource())
            {
                var token = source.Token;

                try
                {
                    this.ConcurrentDictionary.Clear();
                    this.SetCancellationAction(source, request);
                    this.Find(request, token);
                }
                catch (Exception e)
                {
                    if (!source.IsCancellationRequested)
                        source.Cancel();

                    Log.Error(this, e);
                }
            }

            return new SearchableResponse(this.ConcurrentDictionary);
        }

        /// <summary>
        ///     Sets the cancellation action.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="request">The request.</param>
        private void SetCancellationAction(CancellationTokenSource source, TSearchableRequest request)
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

                        case ThresholdConstraints.Package:

                            foreach (var p in request.Packages)
                            {
                                var pkg = p;

                                var items = this.ConcurrentDictionary.Where(o => pkg.Items.Any(item => item.Name.Equals(o.Key)));
                                if (items.Sum(item => item.Value.Count) >= request.Threshold)
                                {
                                    constraints++;
                                    break;
                                }
                            }

                            if (constraints == request.Packages.Count)
                                source.Cancel();

                            break;
                    }
                }
            };
        }

        /// <summary>
        ///     Checks the threshold contraints.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="request">The request.</param>
        /// <returns>Returns a <see cref="bool" /> representing <c>true</c> when the constraint has not be violated.</returns>
        private bool ViolateThresholdContraint(string name, TSearchableRequest request)
        {
            if (request.ThresholdConstraint != ThresholdConstraints.Item)
                return false;

            if (this.ConcurrentDictionary.ContainsKey(name))
                if (this.ConcurrentDictionary[name].Count >= request.Threshold)
                    return true;

            return false;
        }

        #endregion
    }
}