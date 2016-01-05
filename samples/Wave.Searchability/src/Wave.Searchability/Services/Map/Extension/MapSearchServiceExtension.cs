using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework;
using Miner.Geodatabase;
using Miner.Interop;

using Wave.Searchability.Data;

namespace Wave.Searchability.Services
{    
    public class MapSearchServiceExtension : BaseExtension
    {
        public const string ExtensionName = "Wave Search Service Extension";

        private IEventAggregator _EventAggregator;

        public MapSearchServiceExtension()
            : base(ExtensionName)
        {
            _EventAggregator = new EventAggregator();
        }

        /// <summary>
        /// Initialization function for extension
        /// </summary>
        /// <param name="initializationData">ESRI Application Reference</param>
        public override void Startup(ref object initializationData)
        {
            base.Startup(ref initializationData);

            Document.OpenDocument += (sender, e) => this.LoadResources();

            Document.OpenStoredDisplay += (sender, e) => this.LoadResources();            
        }

        private void LoadResources()
        {
            var sets = this.GetSearchableSets(Document.ActiveMap);
            
        }

        /// <summary>
        /// Creates a searchable set for the individual layers and tables in the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableSet}"/> representing an enumeration of sets.</returns>
        private IEnumerable<SearchableSet> GetSearchableSets(IMap map)
        {
            var layers = new SearchableSet("Layers");

            foreach (var layer in map.Where<IFeatureLayer>(layer => layer.Valid))
            {
                var item = new SearchableTable(((IDataset) layer.FeatureClass).Name)
                {
                    Fields = new ObservableCollection<SearchableField>(new[] {new SearchableField()}),
                    LayerDefinition = true,
                    IsFeatureClass = true
                };

                layers.Tables.Add(item);
            }

            var tables = new SearchableSet("Tables");

            foreach (var table in map.GetTables())
            {
                var item = new SearchableTable(((IDataset) table).Name)
                {
                    Fields = new ObservableCollection<SearchableField>(new[] {new SearchableField()}),
                    LayerDefinition = false,
                    IsFeatureClass = false,
                    Relationships = new ObservableCollection<SearchableRelationship>(new[] {new SearchableRelationship()})
                };

                tables.Tables.Add(item);
            }

            return new[] {layers, tables};
        }
        
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
    }
}