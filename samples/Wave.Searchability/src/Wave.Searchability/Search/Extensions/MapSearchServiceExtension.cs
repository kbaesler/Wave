using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework;

using Wave.Searchability.Data;
using Wave.Searchability.Services;

namespace Wave.Searchability.Extensions
{
    [Guid("DB44276A-8C24-4C4E-A6FF-113198EE9DC9")]
    [ProgId("Wave.Searchability.Extensions.MapSearchServiceExtension")]
    [ComVisible(true)]
    public class MapSearchServiceExtension : BaseExtension
    {
        #region Constants

        public const string ExtensionName = "Wave Search Service Extension";

        #endregion

        #region Constructors

        public MapSearchServiceExtension()
            : base(ExtensionName)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initialization function for extension
        /// </summary>
        /// <param name="initializationData">ESRI Application Reference</param>
        public override void Startup(ref object initializationData)
        {
            base.Startup(ref initializationData);

            Document.OpenDocument += (sender, e) => this.LoadResources();

            Document.OpenStoredDisplay += (sender, e) => this.LoadResources();
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

        #region Private Methods

        
        /// <summary>
        ///     Creates a searchable set for the individual layers and tables in the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableSet}" /> representing an enumeration of sets.</returns>
        private IEnumerable<SearchableSet> GetSearchableSets(IMap map)
        {
            var layers = new SearchableSet("Layers");

            foreach (var layer in map.Where<IFeatureLayer>(layer => layer.Valid))
            {
                var item = new SearchableTable(((IDataset) layer.FeatureClass).Name)
                {
                    LayerDefinition = true,
                    IsFeatureClass = true
                };

                var names = layer.FeatureClass.Fields.AsEnumerable().Select(o => o.Name);
                foreach (var name in names)
                {
                    item.Fields.Add(new SearchableField(name));
                }

                layers.Tables.Add(item);
            }

            var tables = new SearchableSet("Tables");

            foreach (var table in map.GetTables())
            {
                var item = new SearchableTable(((IDataset) table).Name)
                {
                    LayerDefinition = false,
                    IsFeatureClass = false,
                    Relationships = new ObservableCollection<SearchableRelationship>(new[] {new SearchableRelationship()})
                };

                var names = table.Fields.AsEnumerable().Select(o => o.Name);
                foreach (var name in names)
                {
                    item.Fields.Add(new SearchableField(name));
                }

                tables.Tables.Add(item);
            }

            return new[] {layers, tables};
        }

        private void LoadResources()
        {
            var request = new MapSearchServiceRequest
            {
                Items = this.GetSearchableSets(Document.ActiveMap),
                Keywords = "kellyl",
                ComparisonOperator = ComparisonOperator.Like
            };
            
            IMapSearchService service = new MapSearchService();
            service.Find(request);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            service.FindAsync(request).ContinueWith(task =>
            {
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                Log.Debug(this, "{0:N0} ms", elapsedMilliseconds);
                Log.Debug(this, "{0}", task.Result.Count);
            });
        }

        #endregion
    }
}