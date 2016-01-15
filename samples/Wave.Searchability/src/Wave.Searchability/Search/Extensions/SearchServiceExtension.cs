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
using Wave.Searchability.Views;

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
        /// <returns>Returns a <see cref="IEnumerable{SearchableItem}" /> representing an enumeration of sets.</returns>
        private List<SearchableInventory> GetInventory(IMap map)
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

            return sets;
        }

        /// <summary>
        ///     Gets the layer set.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableItem}" /> representing the layers in the map.</returns>
        private IEnumerable<SearchableInventory> GetLayerInventory(IMap map)
        {
            var items = new List<SearchableInventory>(); 

            foreach (IFeatureLayer layer in map.Where<IFeatureLayer>(layer => layer.Valid).DistinctBy(layer => layer.FeatureClass.ObjectClassID))
            {                
                var item = new SearchableLayer(((IDataset) layer.FeatureClass).Name)
                {
                    LayerDefinition = true,
                    Fields = new ObservableCollection<SearchableField>(new[] {new SearchableField()})
                };

                var inventory = new SearchableInventory(item.Name, item);
                items.Add(inventory);
            }

            return items;
        }

        /// <summary>
        ///     Gets the table set.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns>Returns a <see cref="IEnumerable{SearchableItem}" /> representing the layers in the map.</returns>
        private IEnumerable<SearchableInventory> GetTableInventory(IMap map)
        {
            var items = new List<SearchableInventory>();

            foreach (ITable table in map.GetTables().DistinctBy(o => ((IDataset)o).Name))
            {
                var item = new SearchableTable(((IDataset) table).Name)
                {
                    Relationships = new ObservableCollection<SearchableRelationship>(new[] {new SearchableRelationship()}),
                    Fields = new ObservableCollection<SearchableField>(new[] { new SearchableField() })
                };

                var inventory = new SearchableInventory(item.Name, item);
                items.Add(inventory);
            }

            return items;
        }

        #endregion
    }
}