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

            var layers = source.GetFeatureLayers();
            return layers.Where(o => o.FeatureClass.ObjectClassID == table.ObjectClassID);
        }

        /// <summary>
        ///     Returns the valid layers that are in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{IFeatureLayer}" /> representing the layers in the map.</returns>
        public static IEnumerable<IFeatureLayer> GetFeatureLayers(this IMap source)
        {
            return WhereImpl(source, layer => layer.Valid && layer is IFeatureLayer).OfType<IFeatureLayer>();
        }

        /// <summary>
        ///     Gets the hierarchy of the layers in the table of contents.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IHierarchy{ILayer}" /> representing the hierarchical structure of the layers.</returns>
        public static IEnumerable<IHierarchy<ILayer>> GetHierarchy(this IMap source)
        {
            var layers = source.Layers.AsEnumerable();
            return layers.Select(layer => layer.GetHierarchy());
        }        

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ILayer" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumLayer" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the layers from the input source.</returns>
        public static IEnumerable<ILayer> GetLayers(this IEnumLayer source)
        {
            var layers = source.AsEnumerable();
            return WhereImpl(layers, null, 0, -1).Select(o => o.Value);
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
            return WhereImpl(source, layer => layer is TLayer && selector((TLayer) layer)).OfType<TLayer>();
        }

        /// <summary>
        ///     Returns an enumerable of all of the selectable layers in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureLayer}" /> representing the selectable layers.
        /// </returns>
        public static IEnumerable<IFeatureLayer> GetSelectableLayers(this IMap source)
        {
            if (source == null) return null;

            return WhereImpl(source, layer => layer is IFeatureLayer && ((IFeatureLayer) layer).Selectable).OfType<IFeatureLayer>();
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
        ///     Returns an enumerable of all of the visible layers in the map that satisify the predicate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="selector">A function to test each element for a condition in each visible layer.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureLayer}" /> representing the visible layers.
        /// </returns>
        /// <remarks>
        ///     This method determines if a layer is actually visible in a map.  It does this by checking to see if the layer is
        ///     not drawn due to scale ranges and
        ///     also by validating whether or not the layer is in a composite layer or group layer that is not visible.
        /// </remarks>
        public static IEnumerable<IFeatureLayer> GetVisibleLayers(this IMap source, Func<IFeatureLayer, bool> selector)
        {
            if (source == null) return null;

            var layers = WhereImpl(source, layer => source.IsLayerVisible(layer) && layer is IFeatureLayer).OfType<IFeatureLayer>();
            return layers.Where(selector);
        }

        /// <summary>
        ///     Returns an enumerable of all of the visible layers in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureLayer}" /> representing the visible layers.
        /// </returns>
        /// <remarks>
        ///     This method determines if a layer is actually visible in a map.  It does this by checking to see if the layer is
        ///     not drawn due to scale ranges and
        ///     also by validating whether or not the layer is in a composite layer or group layer that is not visible.
        /// </remarks>
        public static IEnumerable<IFeatureLayer> GetVisibleLayers(this IMap source)
        {
            if (source == null) return null;

            return WhereImpl(source, layer => source.IsLayerVisible(layer) && layer is IFeatureLayer).OfType<IFeatureLayer>();
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

            var layers = WhereImpl(source, layer => predicate(layer) && layer is IFeatureLayer);
            return layers.OfType<IFeatureLayer>().Select(o => ((IDataset) o.FeatureClass).Workspace).FirstOrDefault();
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
            return WhereImpl(source, selector);
        }

        #endregion

        #region Private Methods

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
        private static IEnumerable<ILayer> WhereImpl(IMap source, Func<ILayer, bool> selector)
        {
            if (source == null) return null;
            if (selector == null) throw new ArgumentNullException("selector");

            var layers = source.Layers.AsEnumerable();
            return WhereImpl(layers, selector, 0, -1).Select(o => o.Value);
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> selecting only those layers that satisfy the
        ///     <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The map.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="maximum">The maximum.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{TLayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">selector</exception>
        private static IEnumerable<IRecursion<ILayer>> WhereImpl(IEnumerable<ILayer> source, Func<ILayer, bool> selector, int depth, int maximum)
        {
            if (selector == null) throw new ArgumentNullException("selector");

            depth++;

            foreach (var child in source)
            {
                if (selector(child))
                    yield return new Recursion<ILayer>(depth, child);

                if ((depth <= maximum) || (maximum == Recursion<ILayer>.Infinity))
                {
                    var layer = child as ICompositeLayer;
                    if (layer != null)
                    {
                        var children = layer.AsEnumerable();
                        foreach (var item in WhereImpl(children, selector, depth, maximum))
                            yield return item;
                    }
                }
            }
        }

        #endregion
    }
}