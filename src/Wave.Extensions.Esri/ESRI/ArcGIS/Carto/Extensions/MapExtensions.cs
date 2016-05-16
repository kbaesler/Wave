using System;
using System.Collections.Generic;
using System.Linq;

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
        ///     Creates an <see cref="IEnumerable{T}" /> from a <see cref="IMapLayerInfos" />
        /// </summary>
        /// <param name="source">Map layer informations</param>
        /// <returns><see cref="IEnumerable{T}" /> of <see cref="IMapLayerInfo" /></returns>
        public static IEnumerable<IMapLayerInfo> AsEnumerable(this IMapLayerInfos source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    yield return source.Element[i];
                }
            }
        }

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

            return source.GetLayers<IFeatureLayer>(o => o.FeatureClass.ObjectClassID == table.ObjectClassID);
        }

        /// <summary>
        ///     Returns the valid layers that are in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{IFeatureLayer}" /> representing the layers in the map.</returns>
        public static IEnumerable<IFeatureLayer> GetFeatureLayers(this IMap source)
        {
            return source.GetLayers<IFeatureLayer>(layer => layer.Valid);
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
        public static IEnumerable<TLayer> GetLayers<TLayer>(this IMaps source, Func<TLayer, bool> selector)
            where TLayer : ILayer
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            return source.AsEnumerable().SelectMany(map => map.GetLayers(selector));
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
        public static IEnumerable<TLayer> GetLayers<TLayer>(this IMap source, Func<TLayer, bool> selector)
            where TLayer : ILayer
        {
            return source.WhereImp(selector);
        }

        /// <summary>
        ///     Returns the stand-alone tables that are in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{ITable}" /> representing the tables in the map.</returns>
        public static IEnumerable<ITable> GetTables(this IMap source)
        {
            var standaloneTableCollection = source as IStandaloneTableCollection;
            if (standaloneTableCollection != null)
            {
                for (int i = 0; i < standaloneTableCollection.StandaloneTableCount; i++)
                {
                    IStandaloneTable standaloneTable = standaloneTableCollection.StandaloneTable[i];
                    if (standaloneTable != null && standaloneTable.Valid)
                    {
                        yield return standaloneTable.Table;
                    }
                }
            }
        }

        /// <summary>
        ///     Returns an enumerable of all of the visible layers in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureLayer}" /> representing the visible layers.
        /// </returns>
        public static IEnumerable<IFeatureLayer> GetVisibleLayers(this IMap source)
        {
            if (source == null) return null;

            return WhereImp<IFeatureLayer>(source, source.IsLayerVisible);
        }

        /// <summary>
        ///     Returns the workspace for the first occurance of a valid feature layer in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace.
        /// </returns>
        public static IWorkspace GetWorkspace(this IMap source, Predicate<ILayer> predicate)
        {
            if (source.LayerCount == 0)
                return null;

            return WhereImp<IFeatureLayer>(source, layer => predicate(layer)).Select(o => ((IDataset) o.FeatureClass).Workspace).FirstOrDefault();
        }

        /// <summary>
        ///     Returns the workspace for the first occurance of a valid table in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace.
        /// </returns>
        public static IWorkspace GetWorkspace(this IMap source, Predicate<ITable> predicate)
        {
            return source.GetTables().Select(o => ((IDataset) o).Workspace).FirstOrDefault();
        }

        /// <summary>
        ///     Returns the workspace for the first occurance of a valid feature layer in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IWorkspace" /> representing the workspace.
        /// </returns>
        public static IWorkspace GetWorkspace(this IMap source)
        {
            IWorkspace workspace = source.GetWorkspace(layer => layer.Valid);
            if (workspace == null)
            {
                var tables = source.GetTables();
                return tables.Select(table => ((IDataset) table).Workspace).FirstOrDefault();
            }

            return workspace;
        }

        /// <summary>
        ///     Determines whether the specified layer is visible in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the layer is visible.</returns>
        /// <remarks>
        ///     It does this by checking to see if the layer is not drawn due to scale ranges and also by validating whether
        ///     or not the layer is in a composite layer or group layer that is not visible.
        /// </remarks>
        public static bool IsLayerVisible(this IMap source, ILayer layer)
        {
            IMapLayers2 layers = (IMapLayers2) source;
            return layers.IsLayerVisible(layer);
        }

        /// <summary>
        ///     Determines whether the specified parent of the layer is visible in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the parent layer is visible.</returns>
        public static bool IsParentVisible(this IMap source, ILayer layer)
        {
            bool isLayerVisible;
            bool isParentVisible;

            IMapLayers2 layers = (IMapLayers2) source;
            layers.IsLayerVisibleEx(layer, out isLayerVisible, out isParentVisible);
            return isParentVisible;
        }


        /// <summary>
        ///     Traverses the <paramref name="source" /> selecting only those layers that satisfy the
        ///     <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{TLayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">selector</exception>
        /// <exception cref="System.NotSupportedException">The layer type is not supported.</exception>
        public static IEnumerable<ILayer> Where(this IMap source, Func<ILayer, bool> selector)
        {
            return source.WhereImp(selector);
        }

        #endregion

        #region Private Methods

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
        private static IEnumerable<TLayer> WhereImp<TLayer>(this IMap source, Func<TLayer, bool> selector)
            where TLayer : ILayer
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            IEnumLayer layers = source.Layers[null];
            return layers.AsEnumerable().OfType<TLayer>().Where(selector);
        }

        #endregion
    }
}