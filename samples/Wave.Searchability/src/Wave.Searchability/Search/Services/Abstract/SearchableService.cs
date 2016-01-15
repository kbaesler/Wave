using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework;
using Miner.Interop;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{
    /// <summary>
    ///     A service contract for searching the active map session for table(s), class(es) and relationship(s).
    /// </summary>
    [ServiceContract]
    public interface ISearchableService<in TSearchableRequest> where TSearchableRequest : SearchableRequest
    {
        #region Public Methods

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="tables">The tables.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        SearchableResponse Find(TSearchableRequest request, List<IFeatureLayer> layers, List<ITable> tables);

        /// <summary>
        ///     Searches the active map using the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="map">The map.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the results.
        /// </returns>
        SearchableResponse Find(TSearchableRequest request, IMap map);

        /// <summary>
        ///     Searches the active map using the specified request.
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
    public class SearchableService<TSearchableRequest> : ISearchableService<TSearchableRequest>
        where TSearchableRequest : SearchableRequest, new()
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchableService{TSearchableRequest}" /> class.
        /// </summary>
        public SearchableService()
        {
            this.ConcurrentDictionary = new ConcurrentDictionary<string, ConcurrentBag<int>>();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets a value indicating whether there is a pending cancellation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if there is a pending cancellation; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool CancellationPending
        {
            get { return this.ConcurrentDictionary != null && this.Threshold > 0 && this.ConcurrentDictionary.Count >= this.Threshold; }
        }

        /// <summary>
        ///     Gets or sets the concurrent dictionary.
        /// </summary>
        /// <value>
        ///     The concurrent dictionary.
        /// </value>
        protected ConcurrentDictionary<string, ConcurrentBag<int>> ConcurrentDictionary { get; set; }

        /// <summary>
        ///     Gets or sets the threshold.
        /// </summary>
        /// <value>
        ///     The threshold.
        /// </value>
        protected int Threshold { get; set; }

        #endregion

        #region ISearchableService<TSearchableRequest> Members

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public SearchableResponse Find(TSearchableRequest request)
        {
            return this.Find(request, Document.ActiveMap);
        }

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="map">The map.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public SearchableResponse Find(TSearchableRequest request, IMap map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            var layers = map.Where<IFeatureLayer>(layer => layer.Valid).DistinctBy(o => o.FeatureClass.ObjectClassID).ToList();
            var tables = map.GetTables().DistinctBy(o => ((IDataset) o).Name).ToList();

            return this.Find(request, layers, tables);
        }

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="tables">The tables.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public SearchableResponse Find(TSearchableRequest request, List<IFeatureLayer> layers, List<ITable> tables)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            this.ConcurrentDictionary.Clear();
            this.SearchLayers(request, layers);
            this.SearchTables(request, tables, layers);

            return new SearchableResponse(this.ConcurrentDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()));
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="tables">The tables.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        public SearchableResponse FindAsync(TSearchableRequest request, List<IFeatureLayer> layers, List<ITable> tables)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            this.ConcurrentDictionary.Clear();

            using (var source = new CancellationTokenSource())
            {
                try
                {
                    Parallel.Invoke(new ParallelOptions()
                    {
                        CancellationToken = source.Token
                    }, () =>
                        this.SearchLayers(request, layers), () =>
                            this.SearchTables(request, tables, layers));
                }
                catch (AggregateException e)
                {
                    source.Cancel();

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
        protected virtual void Add(IRow row, IFeatureLayer layer, bool isFeatureClass, TSearchableRequest request)
        {
            var name = (layer == null) ? ((IDataset) row.Table).Name : ((IDataset) layer.FeatureClass).Name;

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

        #endregion

        #region Private Methods

        /// <summary>
        ///     Associates the specified object with the layer (when specified) otherwise it associates it with the relationship
        ///     path.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="path">The path.</param>
        /// <param name="index">The index.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="request">The request.</param>
        private void Attach(IObject search, IFeatureLayer layer, List<string> path, int index, List<IFeatureLayer> layers, TSearchableRequest request)
        {
            if (path == null) return;
            if (path.Count == 0) return;
            if (index < 0 || index > path.Count) return;

            bool reset = (layer == null);

            IObjectClass searchClass = search.Class;
            IEnumRelationshipClass relationshipClasses = searchClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
            foreach (var relClass in relationshipClasses.AsEnumerable())
            {
                // A stop has been requested or the threshold has been reached.
                if (this.CancellationPending) return;

                // Invalid relationship class.
                if (relClass.RelationshipClassID == -1)
                    continue;

                IDataset dataset = (IDataset) relClass;

                // When the path names match continue.
                if (!dataset.Name.Equals(path[index], StringComparison.CurrentCultureIgnoreCase)
                    && !path[index].Equals(Searchable.Any))
                    continue;

                // Obtain the related objects.
                ISet set = relClass.GetObjectsRelatedToObject(search);
                set.Reset();

                // There are no related objects continue to next
                if (set.Count == 0)
                    continue;

                // When the layer is not provided attempt to find it.
                if (layer == null)
                    layer = this.GetRelationshipLayer(searchClass, relClass, layers);

                if (layer == null)
                    continue;

                IObjectClass layerClass = layer.FeatureClass;

                // Iterate through the object locating the related layer.
                foreach (var obj in set.AsEnumerable<IObject>())
                {
                    if (obj.Class == layerClass)
                    {
                        // Add the related object layer.
                        this.Add(obj, layer, true, request);
                    }
                    else
                    {
                        // Move up the path to the next relationship.
                        this.Attach(obj, layer, path, index - 1, layers, request);
                    }

                    // A stop has been requested or the threshold has been reached.
                    if (this.CancellationPending) return;
                }

                // Reset the layer when it was provided to the method empty.
                if (reset) layer = null;
            }
        }


        /// <summary>
        ///     Gets the layer that is associated to the specified relationship.
        /// </summary>
        /// <param name="searchClass">The search class.</param>
        /// <param name="relClass">The rel class.</param>
        /// <param name="sources">The sources.</param>
        /// <returns>
        ///     The <see cref="IFeatureLayer" /> that is associated to the relationship; otherwise <c>null</c>.
        /// </returns>
        private IFeatureLayer GetRelationshipLayer(IObjectClass searchClass, IRelationshipClass relClass, IEnumerable<IFeatureLayer> sources)
        {
            // Used to make sure the layers belong to the same workspace.
            IMMWorkspaceManager manager = new MMWorkspaceManager();

            // Make sure we have a valid layer in the map.
            IWorkspace targetWorkspace = ((IDataset) relClass).Workspace;

            // Make sure we are not searching the same class.
            IObjectClass oclass = (searchClass == relClass.OriginClass) ? relClass.DestinationClass : relClass.OriginClass;
            string name = ((IDataset) oclass).Name;

            var layers = sources.Where(layer => ((IDataset) layer.FeatureClass).Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            foreach (IFeatureLayer layer in layers)
            {
                IWorkspace sourceWorkspace = ((IDataset) layer.FeatureClass).Workspace;
                if (!manager.IsSameDatabase(targetWorkspace, sourceWorkspace))
                    continue;

                return layer;
            }

            return null;
        }

        /// <summary>
        ///     Searches the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="item">The item.</param>
        /// <param name="request">The request.</param>
        private void SearchLayer(IFeatureLayer layer, SearchableLayer item, TSearchableRequest request)
        {
            IFeatureClass searchClass = layer.FeatureClass;
            var expression = this.CompileExpression((ITable) searchClass, request, item);

            if (string.IsNullOrEmpty(expression))
                return;

            IQueryFilter filter = new QueryFilterClass()
            {
                WhereClause = expression
            };

            if (item.LayerDefinition)
            {
                IFeatureLayerDefinition featureLayerDefinition = (IFeatureLayerDefinition) layer;
                if (!string.IsNullOrEmpty(featureLayerDefinition.DefinitionExpression))
                    filter.WhereClause = string.Format("({0}) {1} ({2})", expression, LogicalOperator.And, featureLayerDefinition.DefinitionExpression);
            }

            foreach (var feature in searchClass.Fetch(filter))
            {
                if (this.CancellationPending)
                    return;

                this.Add(feature, layer, true, request);
            }
        }

        /// <summary>
        ///     Searches the layers.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        private void SearchLayers(TSearchableRequest request, List<IFeatureLayer> layers)
        {
            var items = request.Items.OfType<SearchableLayer>().ToList();
            items.AsParallel().ForAll((item) =>
            {
                foreach (var l in layers.Where(o => ((IDataset) o.FeatureClass).Name.Equals(item.Name) || (item.NameAsClassModelName && o.FeatureClass.IsAssignedClassModelName(item.Name))))
                {
                    var layer = l;

                    Parallel.Invoke(() =>
                        this.SearchLayer(layer, item, request), () =>
                            this.TraverseRelationships(layer.FeatureClass, null, null, item.Relationships, request, layers));

                    if (this.CancellationPending) return;
                }
            });
        }

        /// <summary>
        ///     Searches the relationship.
        /// </summary>
        /// <param name="searchClass">The search class.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="item">The item.</param>
        /// <param name="relationship">The relationship.</param>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        private void SearchRelationship(IObjectClass searchClass, IFeatureLayer layer, SearchableItem item, SearchableRelationship relationship, TSearchableRequest request, List<IFeatureLayer> layers)
        {
            var expression = this.CompileExpression((ITable) searchClass, request, relationship);
            if (string.IsNullOrEmpty(expression))
                return;

            IQueryFilter filter = new QueryFilterClass()
            {
                WhereClause = expression
            };

            List<string> path = item != null ? item.Path : new List<string>(new[] {relationship.Name});

            foreach (var row in ((ITable) searchClass).Fetch(filter))
            {
                if (this.CancellationPending)
                    return;

                this.Attach((IObject) row, layer, path, path.Count - 1, layers, request);
            }
        }

        /// <summary>
        ///     Searches the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="item">The item.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="request">The request.</param>
        private void SearchTable(ITable table, SearchableItem item, List<IFeatureLayer> layers, TSearchableRequest request)
        {
            var expression = this.CompileExpression(table, request, item);
            if (string.IsNullOrEmpty(expression))
                return;

            IQueryFilter filter = new QueryFilterClass()
            {
                WhereClause = expression
            };

            foreach (var row in table.Fetch(filter))
            {
                if (this.CancellationPending) return;

                if (!item.Relationships.Any())
                {
                    this.Add(row, null, false, request);
                }
                else
                {
                    IObject o = (IObject) row;
                    item.Relationships.AsParallel().ForAll((relationship) =>
                        this.Attach(o, null, relationship.Path, relationship.Path.Count - 1, layers, request));
                }
            }
        }

        /// <summary>
        ///     Searches the tables.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="tables">The tables.</param>
        /// <param name="layers">The layers.</param>
        private void SearchTables(TSearchableRequest request, IEnumerable<ITable> tables, List<IFeatureLayer> layers)
        {
            var items = request.Items.OfType<SearchableTable>().ToList();
            items.AsParallel().ForAll((item) =>
            {
                foreach (var t in tables.Where(o => ((IDataset) o).Name.Equals(item.Name) || (item.NameAsClassModelName && o.IsAssignedClassModelName(item.Name))))
                {
                    var table = t;

                    Parallel.Invoke(() =>
                        this.SearchTable(table, item, layers, request), () =>
                            this.TraverseRelationships((IObjectClass) table, null, null, item.Relationships, request, layers));

                    if (this.CancellationPending) return;
                }
            });
        }

        /// <summary>
        ///     Traverses the relationships.
        /// </summary>
        /// <param name="objectClass">The search class.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="item">The item.</param>
        /// <param name="relationships">The relationships.</param>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        private void TraverseRelationships(IObjectClass objectClass, IFeatureLayer layer, SearchableItem item, IEnumerable<SearchableRelationship> relationships, TSearchableRequest request, List<IFeatureLayer> layers)
        {
            if (objectClass == null || relationships == null)
                return;

            foreach (var r in relationships)
            {
                var relationship = r;

                IEnumRelationshipClass relationshipClasses = objectClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
                foreach (var relationshipClass in relationshipClasses.AsEnumerable())
                {
                    if (this.CancellationPending)
                        return;

                    string name = ((IDataset) relationshipClass).Name;
                    if (relationship.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) || r.Name.Equals(Searchable.Any))
                    {
                        IObjectClass searchClass = objectClass == relationshipClass.OriginClass ? relationshipClass.DestinationClass : relationshipClass.OriginClass;

                        Parallel.Invoke(() =>
                            this.SearchRelationship(searchClass, layer, item, relationship, request, layers), () =>
                                this.TraverseRelationships(searchClass, layer, relationship, relationship.Relationships, request, layers));

                        if (this.CancellationPending)
                            return;
                    }
                }
            }
        }

        #endregion
    }
}