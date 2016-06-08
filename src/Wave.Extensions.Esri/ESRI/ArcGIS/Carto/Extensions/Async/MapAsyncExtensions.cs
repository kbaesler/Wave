#if NET45
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Carto
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IMap" /> interface.
    /// </summary>
    public static class MapAsyncExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns the layer that is associated with the <paramref name="table" /> that resides the maps.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="table">The feature class.</param>
        /// <returns>
        ///     Returns the <see cref="IFeatureLayer" /> representing the layer is associated with the feature class.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">table</exception>
        public static Task<IFeatureLayer> GetFeatureLayerAsync(this IMap source, IFeatureClass table)
        {
            return Task.Run(() => source.GetFeatureLayer(table));
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
        public static Task<IEnumerable<IFeatureLayer>> GetFeatureLayersAsync(this IMap source, IFeatureClass table)
        {
            return Task.Run(() => source.GetFeatureLayersAsync(table));
        }

        /// <summary>
        ///     Returns the valid layers that are in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{IFeatureLayer}" /> representing the layers in the map.</returns>
        public static Task<IEnumerable<IFeatureLayer>> GetFeatureLayersAsync(this IMap source)
        {
            return Task.Run(() => source.GetFeatureLayersAsync());
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> selecting only those <see cref="IFeatureLayer" /> that satisfy the
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
        public static Task<IEnumerable<TLayer>> GetLayersAsync<TLayer>(this IMaps source, Func<TLayer, bool> selector)
            where TLayer : ILayer
        {
            return Task.Run(() => source.GetLayersAsync(selector));
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
        public static Task<IEnumerable<TLayer>> GetLayersAsync<TLayer>(this IMap source, Func<TLayer, bool> selector)
            where TLayer : ILayer
        {
            return Task.Run(() => source.GetLayersAsync(selector));
        }

        /// <summary>
        ///     Returns the stand-alone tables that are in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{ITable}" /> representing the tables in the map.</returns>
        public static Task<IEnumerable<ITable>> GetTablesAsync(this IMap source)
        {
            return Task.Run(() => source.GetTables());
        }

        /// <summary>
        ///     Returns the workspace for the first occurance of a valid feature layer in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace.
        /// </returns>
        public static Task<IWorkspace> GetWorkspaceAsync(this IMap source, Predicate<ILayer> predicate)
        {
            return Task.Run(() => source.GetWorkspace(predicate));
        }

        /// <summary>
        ///     Returns the workspace for the first occurance of a valid table in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace.
        /// </returns>
        public static Task<IWorkspace> GetWorkspaceAsync(this IMap source, Predicate<ITable> predicate)
        {
            return Task.Run(() => source.GetWorkspace(predicate));
        }

        /// <summary>
        ///     Returns the workspace for the first occurance of a valid feature layer in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace.
        /// </returns>
        public static Task<IWorkspace> GetWorkspaceAsync(this IMap source)
        {
            return Task.Run(() => source.GetWorkspace());
        }

        #endregion
    }
}

#endif