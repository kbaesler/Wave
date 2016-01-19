using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using System.Windows;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Framework;

using Wave.Searchability.Data;
using Wave.Searchability.Services;

namespace Wave.Searchability.Extensions
{
    [Guid("DB44276A-8C24-4C4E-A6FF-113198EE9DC9")]
    [ProgId("Wave.Searchability.Extensions.SearchServiceExtension")]
    [ComVisible(true)]
    public class SearchServiceExtension : BaseExtensionBootstrap
    {
        #region Constants

        public const string ExtensionName = "Wave Searchability Extension";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SearchServiceExtension" /> class.
        /// </summary>
        public SearchServiceExtension()
            : base(ExtensionName)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Cleanup function for extension.
        /// </summary>
        public override void Shutdown()
        {
            base.Shutdown();
        }

        /// <summary>
        ///     Initialization function for extension
        /// </summary>
        /// <param name="initializationData">ESRI Application Reference</param>
        public override void Startup(ref object initializationData)
        {
            base.Startup(ref initializationData);

            var eventAggregator = this.GetService<IEventAggregator>();
            var searchService = this.GetService<IMapSearchService>();

            Document.OpenStoredDisplay += (sender, e) =>
            {
                var sets = this.GetInventory(Document.ActiveMap);
                eventAggregator.GetEvent<CompositePresentationEvent<IEnumerable<SearchableInventory>>>().Publish(sets);
            };

            eventAggregator.GetEvent<CompositePresentationEvent<MapSearchServiceRequest>>().Subscribe((request) =>
            {
                var response = searchService.Find(request, Document.ActiveMap);
                eventAggregator.GetEvent<CompositePresentationEvent<SearchableResponse>>().Publish(response);
            });
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified CLSID.
        /// </summary>
        /// <param name="CLSID">The CLSID.</param>
        [ComRegisterFunction]
        internal static void Register(string CLSID)
        {
            MxExtension.Register(CLSID);
        }

        /// <summary>
        ///     Unregisters the specified CLSID.
        /// </summary>
        /// <param name="CLSID">The CLSID.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string CLSID)
        {
            MxExtension.Unregister(CLSID);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Configures the bootstrap extension.
        /// </summary>
        protected override void Configure()
        {
            this.AddService(typeof (IEventAggregator), new EventAggregator());
            this.AddService(typeof (IMapSearchService), new MapSearchService());
        }

        #endregion

        #region Private Methods        

        /// <summary>
        ///     Creates a collection of the <see cref="SearchableInventory" /> objects based on the map and custom searches.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableItem}" /> representing an enumeration of sets.</returns>
        private IEnumerable<SearchableInventory> GetInventory(IMap map)
        {
            var sets = new List<SearchableInventory>();

            Parallel.Invoke(() =>
            {
                var layers = this.GetLayerInventory(map);
                sets.AddRange(layers);
            }, () =>
            {
                var tables = this.GetTableInventory(map);
                sets.AddRange(tables);
            });

            return sets.OrderBy(o => o.Name);
        }
         
        /// <summary>
        ///     Gets the layer inventory.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableItem}" /> representing the layers in the map.</returns>
        private IEnumerable<SearchableInventory> GetLayerInventory(IMap map)
        {
            var items = new List<SearchableInventory>();

            foreach (IFeatureLayer layer in map.Where<IFeatureLayer>(layer => layer.Valid).DistinctBy(layer => layer.FeatureClass.ObjectClassID))
            {
                var item = new SearchableLayer(layer.Name, layer.FeatureClass.AliasName)
                {
                    LayerDefinition = !string.IsNullOrEmpty(((IFeatureLayerDefinition)layer).DefinitionExpression),
                    Fields = new ObservableCollection<SearchableField>(new[] {new SearchableField()}),
                };

                var inventory = new SearchableInventory(item.Name, layer.Name, item)
                {
                    Type = this.GetInventoryType(layer.FeatureClass.ShapeType),
                    Header = "Layers"
                };
                items.Add(inventory);
            }

            return items;
        }

        /// <summary>
        ///     Gets the table inventory.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableItem}" /> representing the layers in the map.</returns>
        private IEnumerable<SearchableInventory> GetTableInventory(IMap map)
        {
            var items = new List<SearchableInventory>();

            foreach (ITable table in map.GetTables().DistinctBy(o => ((IDataset) o).Name))
            {
                var aliasName = ((IObjectClass) table).AliasName;
                var item = new SearchableTable(((IDataset) table).Name, aliasName)
                {
                    Relationships = new ObservableCollection<SearchableRelationship>(new[] {new SearchableRelationship()}),
                    Fields = new ObservableCollection<SearchableField>(new[] {new SearchableField()})
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

        /// <summary>
        /// Gets the type of the inventory.
        /// </summary>
        /// <param name="geometryType">Type of the geometry.</param>
        /// <returns>Returns a <see cref="SearchableInventoryType"/> representing the type for the geometry.</returns>
        private SearchableInventoryType GetInventoryType(esriGeometryType geometryType)
        {
            switch (geometryType)
            {
                case esriGeometryType.esriGeometryLine:
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryPath:
                    return SearchableInventoryType.Line;

                case esriGeometryType.esriGeometryMultipoint:
                case esriGeometryType.esriGeometryPoint:
                    return SearchableInventoryType.Point;

                default:
                    return SearchableInventoryType.Polygon;
            }
        }

        #endregion
    }
}