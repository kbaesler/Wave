using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        #endregion
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
        #region ITextSearchService<TSearchableRequest> Members

        /// <summary>
        ///     Searches the data source using the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the results.
        /// </returns>
        public override SearchableResponse Find(TSearchableRequest request, IMap source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var layers = source.Where<IFeatureLayer>(layer => layer.Valid).DistinctBy(o => o.FeatureClass.ObjectClassID).ToList();
            var tables = source.GetTables().DistinctBy(o => ((IDataset) o).Name).ToList();

            return this.Find(request, layers, tables);
        }

        /// <summary>
        ///     Searches the active data source using the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableResponse" /> representing the results.
        /// </returns>
        public override SearchableResponse Find(TSearchableRequest request)
        {
            return this.Find(request, Document.ActiveMap);
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

    /// <summary>
    ///     The requests that are issued to the searchable service.
    /// </summary>
    [DataContract]
    public class TextSearchServiceRequest : SearchableRequest
    {
    }
}