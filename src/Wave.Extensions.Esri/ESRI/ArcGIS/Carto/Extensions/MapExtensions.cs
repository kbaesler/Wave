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
        ///     Returns the layer that is associated with the <paramref name="featureClass" /> that resides the maps.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="featureClass">The feature class.</param>
        /// <returns>
        ///     Returns the <see cref="IFeatureLayer" /> representing the layer is associated with the feature class.
        /// </returns>
        public static IFeatureLayer GetFeatureLayer(this IMap source, IFeatureClass featureClass)
        {
            var list = source.GetFeatureLayers(featureClass);
            return list.FirstOrDefault();
        }

        /// <summary>
        ///     Returns the layers that are associated with the <paramref name="featureClass" /> that resides the map.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="featureClass">The feature class.</param>
        /// <returns>
        ///     Returns the <see cref="IEnumerable{IFeatureLayer}" /> representing the layers are associated with the feature
        ///     class.
        /// </returns>
        public static IEnumerable<IFeatureLayer> GetFeatureLayers(this IMap source, IFeatureClass featureClass)
        {
            return source.Where(o => o.Valid && o.FeatureClass.ObjectClassID == featureClass.ObjectClassID);
        }


        /// <summary>
        ///     Traverses the <paramref name="source" /> selecting only those <see cref="IFeatureLayer" /> that satisifies the
        ///     <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{IFeatureLayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IFeatureLayer> Where(this IMaps source, Func<IFeatureLayer, bool> selector)
        {
            return source.AsEnumerable().SelectMany(map => map.Where(selector));
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> selecting only those <see cref="IFeatureLayer" /> that satisifies the
        ///     <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{IFeatureLayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<IFeatureLayer> Where(this IMap source, Func<IFeatureLayer, bool> selector)
        {
            if (source == null)
                return null;

            UID uid = new UIDClass();
            uid.Value = LayerExtensions.Guid.FeatureLayers;

            IEnumLayer layers = source.Layers[uid];
            return layers.AsEnumerable().OfType<IFeatureLayer>().Where(selector);
        }

        #endregion
    }
}