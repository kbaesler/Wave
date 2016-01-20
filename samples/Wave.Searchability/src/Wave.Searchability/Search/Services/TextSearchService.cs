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
using Miner.Interop;

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
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="source"></param>
        /// <param name="token"></param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        protected override void Find(TSearchableRequest request, IMap source, CancellationToken token)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var layers = source.GetLayers<IFeatureLayer>(layer => layer.Valid).ToList();
            var tables = source.GetTables().DistinctBy(o => ((IDataset) o).Name).ToList();

            this.Find(request, layers, tables, token);
        }

        /// <summary>
        ///     Searches the active map using the specified <paramref name="request" /> for the specified keywords.
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

            Parallel.Invoke(new ParallelOptions() {CancellationToken = token}, () =>
                this.SearchLayers(request, layers, token), () =>
                    this.SearchTables(request, tables, layers, token));
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
        /// <param name="token">The token.</param>
        private void Attach(IObject search, IFeatureLayer layer, List<string> path, int index, List<IFeatureLayer> layers, TSearchableRequest request, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            if (path == null) return;
            if (path.Count == 0) return;
            if (index < 0 || index > path.Count) return;

            bool reset = (layer == null);

            IObjectClass searchClass = search.Class;
            IEnumRelationshipClass relationshipClasses = searchClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
            foreach (var relClass in relationshipClasses.AsEnumerable())
            {
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
                        this.Add(obj, layer, true, request, token);
                    }
                    else
                    {
                        // Move up the path to the next relationship.
                        this.Attach(obj, layer, path, index - 1, layers, request, token);
                    }
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
        /// <param name="token">The token.</param>
        private void SearchLayer(IFeatureLayer layer, SearchableLayer item, TSearchableRequest request, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            IFeatureClass searchClass = layer.FeatureClass;
            var expression = this.CompileExpression((ITable) searchClass, request, item);

            if (string.IsNullOrEmpty(expression))
                return;

            var filter = this.CreateFilter(layer, expression, item, request);
            foreach (var feature in searchClass.Fetch(filter))
                this.Add(feature, layer, true, request, token);
        }

        /// <summary>
        ///     Searches the layers.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="token">The token.</param>
        private void SearchLayers(TSearchableRequest request, List<IFeatureLayer> layers, CancellationToken token)
        {
            var parallel = new ParallelOptions() {CancellationToken = token};
            var items = request.Inventory.SelectMany(inventory => inventory.Items.OfType<SearchableLayer>()).ToList();
            foreach (var i in items)
            {
                var item = i;

                foreach (var l in layers.Where(o => o.Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase) || (item.NameAsClassModelName && o.FeatureClass.IsAssignedClassModelName(item.Name))))
                {
                    var layer = l;

                    Parallel.Invoke(parallel, () =>
                        this.SearchLayer(layer, item, request, token), () =>
                            this.TraverseRelationships(layer.FeatureClass, null, null, item.Relationships, request, layers, token));

                    if (token.IsCancellationRequested)
                        return;
                }
            }
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
        /// <param name="token">The token.</param>
        private void SearchRelationship(IObjectClass searchClass, IFeatureLayer layer, SearchableItem item, SearchableRelationship relationship, TSearchableRequest request, List<IFeatureLayer> layers, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            var expression = this.CompileExpression((ITable) searchClass, request, relationship);
            if (string.IsNullOrEmpty(expression))
                return;

            IQueryFilter filter = new QueryFilterClass()
            {
                WhereClause = expression
            };

            List<string> path = item != null ? item.Path : new List<string>(new[] {relationship.Name});

            foreach (var row in ((ITable) searchClass).Fetch(filter))
                this.Attach((IObject) row, layer, path, path.Count - 1, layers, request, token);
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

            foreach (var row in table.Fetch(filter))
            {
                if (!item.Relationships.Any())
                {
                    this.Add(row, null, false, request, token);
                }
                else
                {
                    IObject o = (IObject) row;
                    foreach (var relationship in item.Relationships)
                        this.Attach(o, null, relationship.Path, relationship.Path.Count - 1, layers, request, token);
                }
            }
        }

        /// <summary>
        ///     Searches the tables.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="tables">The tables.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="token">The token.</param>
        private void SearchTables(TSearchableRequest request, List<ITable> tables, List<IFeatureLayer> layers, CancellationToken token)
        {
            var parallel = new ParallelOptions() {CancellationToken = token};
            var items = request.Inventory.SelectMany(inventory => inventory.Items.OfType<SearchableTable>()).ToList();
            foreach (var i in items)
            {
                var item = i;

                foreach (var t in tables.Where(o => ((IDataset) o).Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase) || (item.NameAsClassModelName && o.IsAssignedClassModelName(item.Name))))
                {
                    var table = t;

                    Parallel.Invoke(parallel, () =>
                        this.SearchTable(table, item, layers, request, token), () =>
                            this.TraverseRelationships((IObjectClass) table, null, null, item.Relationships, request, layers, token));

                    if (token.IsCancellationRequested)
                        return;
                }
            }
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
        /// <param name="token">The token.</param>
        private void TraverseRelationships(IObjectClass objectClass, IFeatureLayer layer, SearchableItem item, IEnumerable<SearchableRelationship> relationships, TSearchableRequest request, List<IFeatureLayer> layers, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            foreach (var r in relationships)
            {
                var relationship = r;

                IEnumRelationshipClass relationshipClasses = objectClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
                foreach (var relationshipClass in relationshipClasses.AsEnumerable())
                {
                    string name = ((IDataset) relationshipClass).Name;
                    if (relationship.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) || relationship.Name.Equals(Searchable.Any))
                    {
                        IObjectClass searchClass = objectClass == relationshipClass.OriginClass ? relationshipClass.DestinationClass : relationshipClass.OriginClass;

                        this.SearchRelationship(searchClass, layer, item, relationship, request, layers, token);

                        this.TraverseRelationships(searchClass, layer, relationship, relationship.Relationships, request, layers, token);
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