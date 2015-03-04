using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Carto
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMap" /> interface.
    /// </summary>
    public static class MapExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="IMaps" />
        /// </summary>
        /// <param name="source">An <see cref="IMap" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the layers from the input source.</returns>
        public static IEnumerable<IMap> AsEnumerable(this IMaps source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count - 1; i++)
                    yield return source.Item[i];
            }
        }

        /// <summary>
        ///     Returns the layer that is associated with the <paramref name="table" /> that resides the maps.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="table">The feature class.</param>
        /// <returns>
        ///     Returns the <see cref="IFeatureLayer" /> representing the layer is associated with the feature class.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        public static IFeatureLayer GetFeatureLayer(this IMap source, IFeatureClass table)
        {
            if (source == null) return null;
            if (table == null) throw new ArgumentNullException("table");

            var list = source.GetFeatureLayers(table);
            return list.FirstOrDefault();
        }

        /// <summary>
        ///     Returns the layers that are associated with the <paramref name="table" /> that resides the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="table">The feature class.</param>
        /// <returns>
        ///     Returns the <see cref="IEnumerable{IFeatureLayer}" /> representing the layers are associated with the feature
        ///     class.
        /// </returns>
        public static IEnumerable<IFeatureLayer> GetFeatureLayers(this IMap source, IFeatureClass table)
        {
            if (source == null) return null;
            if (table == null) throw new ArgumentNullException("table");

            return source.Where<IFeatureLayer>(o => o.Valid && o.FeatureClass.ObjectClassID == table.ObjectClassID);
        }


        /// <summary>
        ///     Traverses the <paramref name="source" /> selecting only those <see cref="IFeatureLayer" /> that satisfy the
        ///     <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{IFeatureLayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">selector</exception>
        public static IEnumerable<IFeatureLayer> Where(this IMaps source, Func<IFeatureLayer, bool> selector)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.AsEnumerable().SelectMany(map => map.Where(selector));
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> selecting only those layers that satisfy the
        ///     <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TLayer">The type of the layer.</typeparam>
        /// <param name="source">The map.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{TLayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">selector</exception>
        /// <exception cref="System.NotSupportedException">The layer type is not supported.</exception>
        public static IEnumerable<TLayer> Where<TLayer>(this IMap source, Func<TLayer, bool> selector)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            var list = new Dictionary<Type, string>
            {
                {typeof (IFeatureLayer), "{40A9E885-5533-11d0-98BE-00805F7CED21}"},
                {typeof (IFeatureLayer2), "{40A9E885-5533-11d0-98BE-00805F7CED21}"},
                {typeof (IGroupLayer), "{EDAD6644-1810-11D1-86AE-0000F8751720}"},
                {typeof (IDataLayer), "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}"},
                {typeof (IDataLayer2), "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}"},
                {typeof (IGraphicsLayer), "{34B2EF81-F4AC-11D1-A245-080009B6F22B}"},
                {typeof (IFDOGraphicsLayer), "{5CEAE408-4C0A-437F-9DB3-054D83919850}"},
                {typeof (IFDOGraphicsLayer2), "{5CEAE408-4C0A-437F-9DB3-054D83919850}"},
                {typeof (ICoverageAnnotationLayer), "{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}"},
                {typeof (ICoverageAnnotationLayer2), "{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}"},
                {typeof (ILayer), null},
                {typeof (ILayer2), null}
            };

            Type type = typeof (TLayer);
            if (!list.ContainsKey(type))
                throw new NotSupportedException("The type of layer is not supported.");

            UID uid = null;
            if (!string.IsNullOrEmpty(list[type]))
            {
                uid = new UIDClass {Value = list[type]};
            }

            IEnumLayer layers = source.Layers[uid];
            return layers.AsEnumerable().OfType<TLayer>().Where(selector);
        }

        #endregion
    }
}