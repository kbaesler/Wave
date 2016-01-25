using System;
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
        /// Converts to response to the <see cref="IMMRowSearchResults2" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tables">The tables.</param>
        /// <returns>
        /// Returns a <see cref="IMMRowSearchResults2" /> representing the response objects.
        /// </returns>
        public static IMMRowSearchResults2 ToSearchResults(this SearchableResponse source, List<ITable> tables)
        {
            IMMRowSearchResults2 results = new RowSearchResults();
            foreach (var s in source)
            {
                var table = tables.FirstOrDefault(l => (((IDataset)l).Name.Equals(s.Key, StringComparison.CurrentCultureIgnoreCase)));
                if (table != null)
                {
                    using (ComReleaser cr = new ComReleaser())
                    {
                        var oids = s.Value.ToArray();
                        var cursor = table.GetRows(oids, false);
                        cr.ManageLifetime(cursor);

                        results.AddCursor(cursor, false);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Converts to response to the <see cref="IMMRowLayerSearchResults2" /> object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="layers">The layers.</param>
        /// <returns>
        /// Returns a <see cref="IMMRowLayerSearchResults2" /> representing the response objects.
        /// </returns>
        public static IMMRowLayerSearchResults2 ToSearchResults(this SearchableResponse source, List<IFeatureLayer> layers)
        {
            IMMRowLayerSearchResults2 results = new RowLayerSearchResults();
            foreach (var s in source)
            {
                var layer = layers.FirstOrDefault(l => (l.Name.Equals(s.Key, StringComparison.CurrentCultureIgnoreCase)));
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