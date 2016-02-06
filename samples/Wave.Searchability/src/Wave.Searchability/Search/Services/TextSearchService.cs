using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{
    /// <summary>
    ///     A service contract for searching the active map session for table(s), class(es) and relationship(s).
    /// </summary>
    [ServiceContract]
    public interface ITextSearchService : ITextSearchService<TextSearchServiceRequest>
    {
    }

    /// <summary>
    ///     A service contract for searching the active map session for table(s), class(es) and relationship(s).
    /// </summary>
    public interface ITextSearchService<in TSearchableRequest> : ISearchableService<TSearchableRequest, IMap>
        where TSearchableRequest : TextSearchServiceRequest
    {
    }

    /// <summary>
    ///     The text-based search service allows for querying a set of table(s), class(es) and relationship(s) using the data
    ///     in the active session.
    /// </summary>
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class TextSearchService : TextSearchService<TextSearchServiceRequest>
    {
    }

    /// <summary>
    ///     The text-based search service allows for querying a set of table(s), class(es) and relationship(s) using the data
    ///     in the active session.
    /// </summary>
    public abstract class TextSearchService<TSearchableRequest> : SearchableService<TSearchableRequest, IMap>, ITextSearchService<TSearchableRequest>
        where TSearchableRequest : TextSearchServiceRequest, new()
    {
        #region Protected Methods

        /// <summary>
        ///     Compiles the filter that is used to query the feature layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="item">The item.</param>
        /// <param name="request">The request.</param>
        /// <returns>Return <see cref="IQueryFilter" /> representing the filter.</returns>
        protected virtual IQueryFilter CreateFilter(IFeatureLayer layer, string expression, SearchableLayer item, TSearchableRequest request)
        {
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

            return filter;
        }

        /// <summary>
        ///     Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        protected override void Find(TSearchableRequest request, IMap data, CancellationToken token)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var layers = data.GetLayers<IFeatureLayer>(layer => layer.Valid).ToList();
            var tables = data.GetTables().DistinctBy(o => ((IDataset) o).Name).ToList();

            this.Find(request, layers, tables, token);
        }


        /// <summary>
        ///     Searches for the results using the specified <paramref name="request" />.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token"></param>
        protected override void Find(TSearchableRequest request, CancellationToken token)
        {
            this.Find(request, Document.ActiveMap, token);
        }

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="tables">The tables.</param>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the contents of the results.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        protected void Find(TSearchableRequest request, List<IFeatureLayer> layers, List<ITable> tables, CancellationToken token)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            //Parallel.Invoke(new ParallelOptions() {CancellationToken = token}, () =>
            //    this.SearchLayers(request, layers, token), () =>
            //        this.SearchTables(request, tables, layers, token));

            this.SearchLayers(request, layers, token);
            this.SearchTables(request, tables, layers, token);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Associates the specified object with the layer (when specified) otherwise it associates it with the relationship
        ///     path.
        /// </summary>
        /// <param name="searchClass">The search class.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="relationships">The relationships.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The token.</param>
        private void Attach(IObjectClass searchClass, ISet rows, IEnumerable<SearchableRelationship> relationships, IFeatureLayer layer, List<IFeatureLayer> layers, TSearchableRequest request, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (rows.Count == 0)
                return;

            IEnumRelationshipClass relationshipClasses = searchClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
            foreach (var item in relationships)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                foreach (var relationshipClass in relationshipClasses.AsEnumerable())
                {
                    var name = ((IDataset) relationshipClass).Name;
                    if (item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)
                        || item.Name.Equals(Searchable.Any))
                    {
                        IObjectClass relClass = (searchClass == relationshipClass.OriginClass) ? relationshipClass.DestinationClass : relationshipClass.OriginClass;
                        bool isFeatureClass = (relClass is IFeatureClass);
                        bool isRecursive = item.Relationships.Any();

                        if (isFeatureClass || isRecursive)
                        {
                            ISet set = relationshipClass.GetObjectsRelatedToObjectSet(rows);
                            set.Reset();

                            if (isRecursive)
                            {
                                this.Attach(relClass, set, item.Relationships, layer, layers, request, cancellationToken);
                            }
                            else
                            {
                                layer = layer ?? layers.FirstOrDefault(l => l.FeatureClass.ObjectClassID == relClass.ObjectClassID);
                                if (layer != null)
                                {
                                    foreach (var row in set.AsEnumerable<IObject>())
                                    {
                                        this.Add(row, layer, true, request, cancellationToken);
                                    }
                                }                               
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Searches the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="item">The item.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The token.</param>
        private void SearchLayer(IFeatureLayer layer, SearchableLayer item, TSearchableRequest request, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            IFeatureClass searchClass = layer.FeatureClass;
            var expression = this.CompileExpression((ITable) searchClass, request, item);

            if (string.IsNullOrEmpty(expression))
                return;

            var filter = this.CreateFilter(layer, expression, item, request);
            foreach (var feature in searchClass.Fetch(filter))
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                this.Add(feature, layer, true, request, cancellationToken);
            }
        }

        /// <summary>
        ///     Searches the layers.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="cancellationToken">The token.</param>
        private void SearchLayers(TSearchableRequest request, List<IFeatureLayer> layers, CancellationToken cancellationToken)
        {
            var parallel = new ParallelOptions() {CancellationToken = cancellationToken};
            var items = request.Inventory.SelectMany(inventory => inventory.Items.OfType<SearchableLayer>()).ToList();
            foreach (var i in items)
            {
                var item = i;

                foreach (var l in layers.Where(o => o.Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase) || (item.NameAsClassModelName && o.FeatureClass.IsAssignedClassModelName(item.Name))))
                {
                    var layer = l;

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    //Parallel.Invoke(parallel, () =>
                    //    this.SearchLayer(layer, item, request, cancellationToken), () =>
                    //        this.TraverseRelationships(layer.FeatureClass, layer, layers, item, request, cancellationToken));

                    this.SearchLayer(layer, item, request, cancellationToken);
                    
                    this.TraverseRelationships(layer.FeatureClass, layer, layers, item, request, cancellationToken);                   
                }
            }
        }

        /// <summary>
        ///     Searches the relationship.
        /// </summary>
        /// <param name="searchClass">The search class.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="item">The item.</param>
        /// <param name="relationship">The relationship.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The token.</param>
        private void SearchRelationship(IObjectClass searchClass, IFeatureLayer layer, List<IFeatureLayer> layers, SearchableItem item, SearchableRelationship relationship, TSearchableRequest request, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var expression = this.CompileExpression((ITable) searchClass, request, relationship);
            if (string.IsNullOrEmpty(expression))
                return;

            IQueryFilter filter = new QueryFilterClass()
            {
                WhereClause = expression
            };

            var rows = new SetClass();
            int rowsAffected = ((ITable) searchClass).Fetch(filter, row => rows.Add(row), false);
            if (rowsAffected > 0)
            {
                this.Attach(searchClass, rows, item.Relationships, layer, layers, request, cancellationToken);
            }
        }

        /// <summary>
        ///     Searches the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="item">The item.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="request">The request.</param>
        /// <param name="token">The token.</param>
        private void SearchTable(ITable table, SearchableItem item, List<IFeatureLayer> layers, TSearchableRequest request, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var expression = this.CompileExpression(table, request, item);
            if (string.IsNullOrEmpty(expression))
                return;

            IQueryFilter filter = new QueryFilterClass()
            {
                WhereClause = expression
            };

            var rows = new SetClass();
            int rowsAffected = table.Fetch(filter, row => rows.Add(row), false);
            if (rowsAffected > 0)
            {
                if (!item.Relationships.Any())
                {
                    foreach (var row in rows.AsEnumerable<IRow>())
                        this.Add(row, null, false, request, token);
                }
                else
                {
                    IObjectClass searchClass = (IObjectClass) table;
                    this.Attach(searchClass, rows, item.Relationships, null, layers, request, token);
                }
            }
        }

        /// <summary>
        ///     Searches the tables.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="tables">The tables.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="cancellationToken">The token.</param>
        private void SearchTables(TSearchableRequest request, List<ITable> tables, List<IFeatureLayer> layers, CancellationToken cancellationToken)
        {
            var parallel = new ParallelOptions() {CancellationToken = cancellationToken};
            var items = request.Inventory.SelectMany(inventory => inventory.Items.OfType<SearchableTable>()).ToList();
            foreach (var i in items)
            {
                var item = i;

                foreach (var t in tables.Where(o => ((IDataset) o).Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase) || (item.NameAsClassModelName && o.IsAssignedClassModelName(item.Name))))
                {
                    var table = t;

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    //Parallel.Invoke(parallel, () =>
                    //    this.SearchTable(table, item, layers, request, cancellationToken), () =>
                    //        this.TraverseRelationships((IObjectClass) table, null, layers, item, request, cancellationToken));

                    this.SearchTable(table, item, layers, request, cancellationToken);
                    
                    this.TraverseRelationships((IObjectClass) table, null, layers, item, request, cancellationToken);                    
                }
            }
        }

        /// <summary>
        ///     Traverses the relationships.
        /// </summary>
        /// <param name="itemClass">The layer class.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="item">The item.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The token.</param>
        private void TraverseRelationships(IObjectClass itemClass, IFeatureLayer layer, List<IFeatureLayer> layers, SearchableItem item, TSearchableRequest request, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            foreach (var r in item.Relationships)
            {
                var relationship = r;

                IEnumRelationshipClass relationshipClasses = itemClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
                foreach (var relationshipClass in relationshipClasses.AsEnumerable())
                {
                    string name = ((IDataset) relationshipClass).Name;
                    if (relationship.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) || relationship.Name.Equals(Searchable.Any))
                    {
                        IObjectClass searchClass = itemClass == relationshipClass.OriginClass ? relationshipClass.DestinationClass : relationshipClass.OriginClass;

                        this.SearchRelationship(searchClass, layer, layers, item, relationship, request, cancellationToken);

                        this.TraverseRelationships(searchClass, layer, layers, relationship, request, cancellationToken);
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     The requests that are issued to the searchable service.
    /// </summary>
    [DataContract]
    public class TextSearchServiceRequest : SearchableRequest
    {
    }
}