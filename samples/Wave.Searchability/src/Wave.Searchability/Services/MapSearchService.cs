using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Wave.Searchability.Data;
using Wave.Searchability.Data.Configuration;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework.BaseClasses;
using Miner.Framework.Search;
using Miner.Interop;

namespace Wave.Searchability.Services
{
    /// <summary>
    /// An implementation of the <see cref="IMMSearchStrategy"/> that allows for 'google-like' search capabilities
    /// in the scense that given a keyword it will search all of the approriate fields in the table or feature classes
    /// specified in the configurations.
    /// </summary>
    public sealed class MapSearchService : BaseSearchStrategy<MapSearchServiceConfiguration>
    {
        #region Protected Properties

        protected override bool ThresholdReached
        {
            get { return this.Results.Threshold > 0 && this.Results.Count >= this.Results.Threshold; }
            set { base.ThresholdReached = value; }
        }

        #endregion

        #region Private Properties

        private Dictionary<int, List<int>> Cache { get; set; }
        private IMMRowLayerSearchResults2 Results { get; set; }

        #endregion

        #region Protected Methods

        protected override IMMSearchResults Find(MapSearchServiceConfiguration configuration)
        {
            this.Results = new RowLayerSearchResults()
            {
                Threshold = configuration.Threshold
            };

            this.SearchLayers(configuration);

            this.SearchTables(configuration);

            return this.Results;
        }

        protected override bool ValidateParameters(MapSearchServiceConfiguration configuration)
        {
            return configuration != null;
        }        

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="layer">The layer.</param>
        private void Add(IRow row, IFeatureLayer layer)
        {
            if (this.Cache == null)
                this.Cache = new Dictionary<int, List<int>>();

            int objectClassId = layer.FeatureClass.ObjectClassID;
            int oid = row.OID;

            if (this.Cache.ContainsKey(objectClassId))
            {
                if (this.Cache[objectClassId].Contains(oid))
                    return;

                this.Cache[objectClassId].Add(oid);
                this.Results.AddRow(row, layer);
            }
            else
            {
                this.Cache.Add(objectClassId, new List<int>(new[] {oid}));
                this.Results.AddRow(row, layer);
            }
        }

        /// <summary>
        ///     Associates the specified object with the layer (when specified) otherwise it associates it with the relationship
        ///     path.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="path">The path.</param>
        /// <param name="index">The index.</param>
        /// <param name="configuration">The parameters.</param>
        private void AttachToLayer(IObject search, IFeatureLayer layer, List<string> path, int index, MapSearchServiceConfiguration configuration)
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
                    && !path[index].Equals("*"))
                    continue;

                // Obtain the related objects.
                ISet set = relClass.GetObjectsRelatedToObject(search);
                set.Reset();

                // There are no related objects continue to next
                if (set.Count == 0)
                    continue;

                // When the layer is not provided attempt to find it.
                if (layer == null)
                    layer = this.GetRelationshipLayer(searchClass, relClass, configuration);

                if (layer == null)
                    continue;

                IObjectClass layerClass = layer.FeatureClass;

                // Iterate through the object locating the related layer.
                IObject obj;
                while ((obj = set.Next() as IObject) != null)
                {
                    if (obj.Class == layerClass)
                    {
                        // Add the related object layer.
                        this.Add(obj, layer);
                    }
                    else
                    {
                        // Move up the path to the next relationship.
                        this.AttachToLayer(obj, layer, path, index - 1, configuration);
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
        /// <param name="configuration">The parameters.</param>
        /// <returns>
        ///     The <see cref="IFeatureLayer" /> that is associated to the relationship; otherwise <c>null</c>.
        /// </returns>
        private IFeatureLayer GetRelationshipLayer(IObjectClass searchClass, IRelationshipClass relClass, MapSearchServiceConfiguration configuration)
        {
            // Used to make sure the layers belong to the same workspace.
            IMMWorkspaceManager manager = new MMWorkspaceManager();

            // Make sure we have a valid layer in the map.
            IWorkspace targetWorkspace = ((IDataset) relClass).Workspace;

            // Make sure we are not searching the same class.
            IObjectClass oclass = (searchClass == relClass.OriginClass) ? relClass.DestinationClass : relClass.OriginClass;
            string name = ((IDataset) oclass).Name;

            var layers = configuration.ActiveMap.Where<IFeatureLayer>(layer => ((IDataset) layer.FeatureClass).Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
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
        /// <param name="filter">The filter.</param>
        /// <param name="configuration">The configuration.</param>
        private void SearchLayer(IFeatureLayer layer, SearchableTable item, IQueryFilter filter, MapSearchServiceConfiguration configuration)
        {
            IFeatureClass queryable = layer.FeatureClass;

            if (item.IsFeatureClass && item.LayerDefinition)
            {
                IFeatureLayerDefinition featureLayerDefinition = (IFeatureLayerDefinition) layer;
                if (!string.IsNullOrEmpty(featureLayerDefinition.DefinitionExpression))
                    filter.WhereClause = string.Format("({0}) {1} ({2})", filter.WhereClause, configuration.LogicalOperator, featureLayerDefinition.DefinitionExpression);
            }

            foreach (var feature in queryable.Fetch(filter))
            {
                if (this.CancellationPending)
                    return;

                this.Add(feature, layer);
            }
        }

        /// <summary>
        ///     Searches the layers.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private void SearchLayers(MapSearchServiceConfiguration configuration)
        {
            var layers = configuration.ActiveMap.Where<IFeatureLayer>(layer => layer.Valid).DistinctBy(o => o.FeatureClass.ObjectClassID).ToList();
            var sets = configuration.Sets.Select(set => set.Tables.Where(table => table.IsFeatureClass));

            foreach (var set in sets)
            {
                foreach (var table in set)
                {
                    var item = table;

                    foreach (var layer in layers.Where(o => ((IDataset) o).Name.Equals(item.Name) || (item.NameAsClassModelName && o.FeatureClass.IsAssignedClassModelName(item.Name))))
                    {
                        var filter = layer.FeatureClass.CreateQuery(configuration.Keywords, configuration.ComparisonOperator, configuration.LogicalOperator, item.Fields.Select(o => o.Name).ToArray());
                        this.SearchLayer(layer, table, filter, configuration);
                    }
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
        /// <param name="configuration">The configuration.</param>
        private void SearchRelationship(IObjectClass searchClass, IFeatureLayer layer, SearchableItem item, SearchableRelationship relationship, MapSearchServiceConfiguration configuration)
        {
            IQueryFilter filter = searchClass.CreateQuery(configuration.Keywords, configuration.ComparisonOperator, configuration.LogicalOperator, relationship.Fields.Select(o => o.Name).ToArray());
            List<string> path = item != null ? item.Path : new List<string>(new[] {relationship.Name});

            foreach (var row in ((ITable) searchClass).Fetch(filter))
            {
                if (this.CancellationPending)
                    return;

                this.AttachToLayer((IObject) row, layer, path, path.Count - 1, configuration);
            }
        }

        /// <summary>
        ///     Searches the table.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <param name="item">The item.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="configuration">The configuration.</param>
        private void SearchTable(ITable queryable, SearchableItem item, IQueryFilter filter, MapSearchServiceConfiguration configuration)
        {
            if (item.Relationships.Count == 0)
            {
                item.Relationships.Add(new SearchableRelationship());
            }

            foreach (var row in queryable.Fetch(filter))
            {
                if (this.CancellationPending) return;

                foreach (var relationship in item.Relationships)
                    this.AttachToLayer((IObject) row, null, relationship.Path, relationship.Path.Count - 1, configuration);
            }
        }

        /// <summary>
        ///     Searches the tables.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private void SearchTables(MapSearchServiceConfiguration configuration)
        {
            var tables = configuration.ActiveMap.GetTables().DistinctBy(o => ((IDataset) o).Name).ToList();
            var sets = configuration.Sets.Select(set => set.Tables.Where(table => !table.IsFeatureClass));

            foreach (var set in sets)
            {
                foreach (var table in set)
                {
                    var item = table;

                    foreach (var standalone in tables.Where(o => ((IDataset) o).Name.Equals(item.Name) || (item.NameAsClassModelName && o.IsAssignedClassModelName(item.Name))))
                    {
                        var searchClass = (IObjectClass) standalone;

                        var filter = searchClass.CreateQuery(configuration.Keywords, configuration.ComparisonOperator, configuration.LogicalOperator, item.Fields.Select(o => o.Name).ToArray());
                        this.SearchTable(standalone, item, filter, configuration);

                        if (this.CancellationPending) return;

                        this.TraverseRelationships(searchClass, null, null, item.Relationships, configuration);
                    }
                }
            }
        }

        /// <summary>
        ///     Traverses the relationships.
        /// </summary>
        /// <param name="searchClass">The search class.</param>
        /// <param name="layer">The layer.</param>
        /// <param name="item">The item.</param>
        /// <param name="relationships">The relationships.</param>
        /// <param name="configuration">The configuration.</param>
        private void TraverseRelationships(IObjectClass searchClass, IFeatureLayer layer, SearchableItem item, IEnumerable<SearchableRelationship> relationships, MapSearchServiceConfiguration configuration)
        {
            if (searchClass == null || relationships == null)
                return;

            foreach (var relationship in relationships)
            {
                if (this.CancellationPending)
                    break;

                IEnumRelationshipClass relationshipClasses = searchClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
                foreach (var relationshipClass in relationshipClasses.AsEnumerable())
                {
                    if (this.CancellationPending)
                        return;

                    string name = ((IDataset) relationshipClass).Name;
                    if (relationship.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) || relationship.Name.Equals("*"))
                    {
                        IObjectClass queryable = searchClass == relationshipClass.OriginClass ? relationshipClass.DestinationClass : relationshipClass.OriginClass;
                        this.SearchRelationship(queryable, layer, item, relationship, configuration);

                        if (this.CancellationPending)
                            return;

                        this.TraverseRelationships(queryable, layer, relationship, relationship.Relationships, configuration);
                    }
                }
            }
        }

        #endregion
    }

    public class MapSearchServiceConfiguration : SearchServiceConfiguration
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the active map.
        /// </summary>
        /// <value>
        ///     The active map.
        /// </value>
        public IMap ActiveMap { get; set; }

        #endregion
    }
}