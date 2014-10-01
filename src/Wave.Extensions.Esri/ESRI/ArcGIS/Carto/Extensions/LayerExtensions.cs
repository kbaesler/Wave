using System;
using System.Collections.Generic;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Carto
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IEnumLayer" /> enumeration.
    /// </summary>
    public static class LayerExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{T}" /> from an <see cref="ILayer" />
        /// </summary>
        /// <param name="source">An <see cref="IEnumLayer" /> to create an <see cref="IEnumerable{T}" /> from.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains the layers from the input source.</returns>
        public static IEnumerable<ILayer> AsEnumerable(this IEnumLayer source)
        {
            if (source != null)
            {
                source.Reset();
                ILayer layer = source.Next();
                while (layer != null)
                {
                    yield return layer;
                    layer = source.Next();
                }
            }
        }

        /// <summary>
        ///     Performs an identify operation with the provided geometry.
        ///     When identifying layers, typically a small envelope is passed in rather than a point to account for differences in
        ///     the precision of the display and the feature geometry.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="trackCancel">
        ///     The cancel tracker object to monitor the Esc key and to terminate processes at the request of
        ///     the user.
        /// </param>
        /// <param name="scale">The current scale of the display.</param>
        /// <returns>Returns a <see cref="IEnumerable{IFeatureIdentifyObj}" /> representing the features that have been identified.</returns>
        /// <remarks>
        ///     On a FeatureIdentifyObject, you can do a QI to the IIdentifyObj interface to get more information about the
        ///     identified feature. The IIdentifyObj interface returns the window handle, layer, and name of the feature; it has
        ///     methods to flash the
        ///     feature in the display and to display a context menu at the Identify location.
        /// </remarks>
        public static IEnumerable<IFeatureIdentifyObj> Identify(this IFeatureLayer source, IGeometry geometry, ITrackCancel trackCancel = null, double scale = -1.0)
        {
            IIdentify2 identify = (IIdentify2) source;
            if (scale > -1.0) identify.Scale = scale;

            IArray array = identify.Identify(geometry, trackCancel);
            if (array != null)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    if (trackCancel != null && !trackCancel.Continue())
                        break;

                    IFeatureIdentifyObj featureIdentify = array.Element[i] as IFeatureIdentifyObj;
                    yield return featureIdentify;
                }
            }
        }

        /// <summary>
        ///     Traverses the <paramref name="source" /> enumeration recursively selecting only those <see cref="ILayer" /> that
        ///     satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">The layers.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{ILayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        public static IEnumerable<ILayer> Where(this IEnumLayer source, Func<ILayer, bool> selector, int depth = -1)
        {
            return WhereImp(source, selector, 0, depth);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Traverses the <paramref name="layers" /> enumeration recursively selecting only those <see cref="ILayer" /> that
        ///     satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <param name="maximum">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{ILayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        private static IEnumerable<ILayer> WhereImp(IEnumLayer layers, Func<ILayer, bool> selector, int depth, int maximum)
        {
            layers.Reset();

            ILayer layer;
            while ((layer = layers.Next()) != null)
            {
                if (selector != null && selector(layer))
                    yield return layer;

                if ((depth <= maximum) || (maximum == -1))
                {
                    ICompositeLayer children = layer as ICompositeLayer;
                    if (children != null)
                    {
                        foreach (var child in WhereImp(children, selector, depth + 1, maximum))
                            yield return child;
                    }
                }
            }
        }

        /// <summary>
        ///     Traverses the <paramref name="layers" /> composite layer recursively selecting only those <see cref="ILayer" />
        ///     that satisify the <paramref name="selector" />
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="selector">A function to test each element for a condition in each recursion.</param>
        /// <param name="depth">The depth of the recursion.</param>
        /// <param name="maximum">The maximum depth of the recursion.</param>
        /// <returns>
        ///     Returns an <see cref="IEnumerable{ILayer}" /> enumeration whose elements
        ///     who are the result of invoking the recursive transform function on each element of the input sequence.
        /// </returns>
        private static IEnumerable<ILayer> WhereImp(ICompositeLayer layers, Func<ILayer, bool> selector, int depth, int maximum)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                ILayer layer = layers.Layer[i];
                if (selector != null && selector(layer))
                    yield return layer;

                if ((depth <= maximum) || (maximum == -1))
                {
                    ICompositeLayer children = layer as ICompositeLayer;
                    if (children != null)
                    {
                        foreach (var child in WhereImp(children, selector, depth + 1, maximum))
                            yield return child;
                    }
                }
            }
        }

        #endregion

        #region Nested Type: Guid

        /// <summary>
        ///     Represents the GUIDs for the common layer types.
        /// </summary>
        public static class Guid
        {
            #region Constants

            /// <summary>
            ///     The <see cref="IDataLayer" /> interface GUID.
            /// </summary>
            public const string DataLayers = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";

            /// <summary>
            ///     The <see cref="IFeatureLayer" /> interface GUID.
            /// </summary>
            public const string FeatureLayers = "{40A9E885-5533-11d0-98BE-00805F7CED21}";

            /// <summary>
            ///     The <see cref="IGraphicsLayer" /> interface GUID.
            /// </summary>
            public const string GraphicsLayers = "{34B2EF81-F4AC-11D1-A245-080009B6F22B}";

            /// <summary>
            ///     The <see cref="IGroupLayer" /> interface GUID.
            /// </summary>
            public const string GroupLayers = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

            #endregion
        }

        #endregion
    }
}