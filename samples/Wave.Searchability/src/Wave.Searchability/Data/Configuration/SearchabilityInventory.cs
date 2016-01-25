using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Newtonsoft.Json;

using Path = System.IO.Path;

namespace Wave.Searchability.Data
{
    public static class SearchabilityInventory
    {
        #region Public Methods

        /// <summary>
        ///     Creates a collection of the <see cref="SearchableInventory" /> objects based on the map and custom searches.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableItem}" /> representing an enumeration of sets.</returns>
        public static IEnumerable<SearchableInventory> GetInventory(IMap map)
        {
            var sets = new List<SearchableInventory>();

            Parallel.Invoke(() =>
            {
                var layers = GetLayerInventory(map);
                sets.AddRange(layers);
            }, () =>
            {
                var tables = GetTableInventory(map);
                sets.AddRange(tables);
            },
                () =>
                {
                    var programData = GetProgramDataInventory();
                    sets.AddRange(programData);
                });

            return sets.OrderBy(o => o.Name);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the type of the inventory.
        /// </summary>
        /// <param name="featureClass">The feature class.</param>
        /// <returns>
        ///     Returns a <see cref="SearchableInventoryType" /> representing the type for the geometry.
        /// </returns>
        private static SearchableInventoryType GetInventoryType(IFeatureClass featureClass)
        {
            var annoClass = featureClass.Extension is IAnnotationClassExtension;
            if (annoClass) return SearchableInventoryType.Annotation;

            var dimClass = featureClass.Extension is IDimensionClassExtension;
            if (dimClass) return SearchableInventoryType.Dimension;

            switch (featureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryLine:
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryPath:
                    return SearchableInventoryType.Line;

                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    return SearchableInventoryType.Point;

                case esriGeometryType.esriGeometryPolygon:
                    return SearchableInventoryType.Polygon;
            }

            return SearchableInventoryType.Unknown;
        }

        /// <summary>
        ///     Gets the layer inventory.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableInventory}" /> representing the layers in the map.</returns>
        private static IEnumerable<SearchableInventory> GetLayerInventory(IMap map)
        {
            var items = new List<SearchableInventory>();

            foreach (IFeatureLayer layer in map.GetLayers<IFeatureLayer>(layer => layer.Valid).DistinctBy(layer => layer.FeatureClass.ObjectClassID))
            {
                var item = new SearchableLayer(layer.Name, layer.FeatureClass.AliasName)
                {
                    LayerDefinition = !string.IsNullOrEmpty(((IFeatureLayerDefinition) layer).DefinitionExpression),
                    Fields = new ObservableCollection<SearchableField>(new[] {new SearchableField()}),
                };

                var inventory = new SearchableInventory(item.Name, layer.Name, item)
                {
                    Type = GetInventoryType(layer.FeatureClass),
                    Header = "Layers"
                };
                items.Add(inventory);
            }

            return items;
        }

        /// <summary>
        ///     Gets the inventory from the program data directory.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<SearchableInventory> GetProgramDataInventory()
        {
            var wave = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Wave");
            if (!Directory.Exists(wave)) Directory.CreateDirectory(wave);

            var searchability = Path.Combine(wave, "Searchability");
            if (!Directory.Exists(searchability)) Directory.CreateDirectory(searchability);

            var inventory = Path.Combine(searchability, "Inventory");
            if (!Directory.Exists(inventory)) Directory.CreateDirectory(inventory);

            foreach (var file in Directory.GetFiles(inventory, "*.json"))
            {
                var json = File.ReadAllText(file);
                yield return JsonConvert.DeserializeObject<SearchableInventory>(json);
            }
        }

        /// <summary>
        ///     Gets the table inventory.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableInventory}" /> representing the layers in the map.</returns>
        private static IEnumerable<SearchableInventory> GetTableInventory(IMap map)
        {
            var items = new List<SearchableInventory>();

            foreach (ITable table in map.GetTables().DistinctBy(o => ((IDataset) o).Name))
            {
                var aliasName = ((IObjectClass) table).AliasName;
                var item = new SearchableTable(((IDataset) table).Name, aliasName)
                {
                    Fields = new ObservableCollection<SearchableField>(new[] {new SearchableField()}),
                    Relationships = new ObservableCollection<SearchableRelationship>(new[] {new SearchableRelationship()})
                };

                var inventory = new SearchableInventory(item.Name, aliasName, item)
                {
                    Type = SearchableInventoryType.Table,
                    Header = "Tables"
                };
                items.Add(inventory);
            }

            return items;
        }

        #endregion
    }
}