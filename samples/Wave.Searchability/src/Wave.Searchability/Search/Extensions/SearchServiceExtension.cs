using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

        #region Fields

        private ServiceHost _ServiceHost;

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

            this.CloseServiceHost();
        }

        /// <summary>
        ///     Initialization function for extension
        /// </summary>
        /// <param name="initializationData">ESRI Application Reference</param>
        public override void Startup(ref object initializationData)
        {
            base.Startup(ref initializationData);

            Document.OpenStoredDisplay += (sender, e) =>
            {
                var sets = GetAllSets(Document.ActiveMap);
                var eventAggregator = GetService<IEventAggregator>();
                eventAggregator.GetEvent<CompositePresentationEvent<IEnumerable<SearchableSet>>>().Publish(sets);
            };
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

            this.CreateServiceHost();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Closes the service host.
        /// </summary>
        private void CloseServiceHost()
        {
            if (_ServiceHost != null)
            {
                _ServiceHost.Close();
                _ServiceHost = null;
            }
        }

        /// <summary>
        ///     Creates the service host.
        /// </summary>
        private void CreateServiceHost()
        {
            _ServiceHost = new ServiceHost(typeof (MapSearchService), new Uri("http://localhost:8000/MapSearchService/"));

            try
            {
                var endPoint = _ServiceHost.AddServiceEndpoint(typeof (IMapSearchService), new WebHttpBinding(), "rest");
                endPoint.Behaviors.Add(new WebHttpBehavior());

                _ServiceHost.Open();
            }
            catch (CommunicationException ce)
            {
                _ServiceHost.Abort();
            }
        }

        /// <summary>
        ///     Creates a collection of the <see cref="SearchableSet" /> objects based on the map and custom searches.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableSet}" /> representing an enumeration of sets.</returns>
        private IEnumerable<SearchableSet> GetAllSets(IMap map)
        {
            var sets = new List<SearchableSet>();

            Parallel.Invoke(() =>
            {
                var layers = this.GetLayerSet(map);
                sets.Add(layers);
            }, () =>
            {
                var tables = this.GetTableSet(map);
                sets.Add(tables);
            });

            return sets;
        }

        /// <summary>
        ///     Gets the layer set.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="SearchableSet" /> representing the layers in the map.</returns>
        private SearchableSet GetLayerSet(IMap map)
        {
            var layers = new SearchableSet("Layers");
            var items = new SortedList(StringComparer.Create(CultureInfo.CurrentCulture, true));

            foreach (IFeatureLayer layer in map.Where<IFeatureLayer>(layer => layer.Valid).DistinctBy(layer => layer.FeatureClass.ObjectClassID))
            {
                var item = new SearchableLayer(((IDataset) layer.FeatureClass).Name)
                {
                    LayerDefinition = true
                };

                IEnumerable<string> names = layer.FeatureClass.Fields.AsEnumerable().Select(o => o.Name);
                foreach (string name in names)
                {
                    item.Fields.Add(new SearchableField(name));
                }

                if (!items.ContainsKey(item.Name))
                    items.Add(item.Name, item);
            }

            layers.Items = new ObservableCollection<SearchableItem>(items.Values.OfType<SearchableItem>());

            return layers;
        }

        /// <summary>
        ///     Gets the table set.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="SearchableSet" /> representing the tables in the map.</returns>
        private SearchableSet GetTableSet(IMap map)
        {
            var tables = new SearchableSet("Tables");
            var items = new SortedList(StringComparer.Create(CultureInfo.CurrentCulture, true));

            foreach (ITable table in map.GetTables())
            {
                var item = new SearchableTable(((IDataset) table).Name)
                {
                    Relationships = new ObservableCollection<SearchableRelationship>(new[] {new SearchableRelationship()})
                };

                IEnumerable<string> names = table.Fields.AsEnumerable().Select(o => o.Name);
                foreach (string name in names)
                {
                    item.Fields.Add(new SearchableField(name));
                }

                if (!items.ContainsKey(item.Name))
                    items.Add(item.Name, item);
            }

            tables.Items = new ObservableCollection<SearchableItem>(items.Values.OfType<SearchableItem>());

            return tables;
        }

        #endregion
    }
}