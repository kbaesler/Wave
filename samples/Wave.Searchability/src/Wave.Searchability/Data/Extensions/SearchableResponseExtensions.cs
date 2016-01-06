using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using Miner.Framework.Search;
using Miner.Interop;

namespace Wave.Searchability.Data
{
    /// <summary>
    /// Provides extension methods for the <see cref="SearchableResponse"/> object.
    /// </summary>
    public static class SearchableResponseExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts to response to the <see cref="IMMRowLayerSearchResults2" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="map">The map.</param>
        /// <returns>
        ///     Returns a <see cref="IMMRowLayerSearchResults2" /> representing the response objects.
        /// </returns>
        public static IMMRowLayerSearchResults2 ToSearchResults(this SearchableResponse source, IMap map)
        {
            return source.ToSearchResults(map.Where<IFeatureLayer>(layer => layer.Valid).ToList());
        }

        /// <summary>
        ///     Converts to response to the <see cref="IMMRowLayerSearchResults2" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="layers">The layers.</param>
        /// <returns>
        ///     Returns a <see cref="IMMRowLayerSearchResults2" /> representing the response objects.
        /// </returns>
        public static IMMRowLayerSearchResults2 ToSearchResults(this SearchableResponse source, List<IFeatureLayer> layers)
        {
            IMMRowLayerSearchResults2 results = new RowLayerSearchResults();

            foreach (var s in source)
            {
                var layer = layers.FirstOrDefault(l => ((IDataset) l.FeatureClass).Name.Equals(s.Key));
                if (layer != null)
                {
                    using (ComReleaser cr = new ComReleaser())
                    {
                        var oids = s.Value.ToArray();
                        var cursor = layer.FeatureClass.GetFeatures(oids, false);
                        cr.ManageLifetime(cursor);

                        results.AddCursor((ICursor) cursor, layer, false);
                    }
                }
            }

            return results;
        }

        #endregion
    }
}