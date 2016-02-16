#if V10
using Miner.Framework;
using ESRI.ArcGIS.Carto;
#endif

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IFeature" /> interface.
    /// </summary>
    public static class FeatureExtensions
    {
        #region Public Methods

#if V10
    /// <summary>
    ///     Flashes the feature for the specified interval (in milliseconds) a set number of times using the color specified by
    ///     the ArcFM Properties.
    /// </summary>
    /// <param name="source">The source that will be flashed.</param>
    /// <param name="interval">The interval in milliseconds.</param>
    /// <param name="flashTimes">Number of times to flash the feature.</param>
        public static void Flash(this IFeature source, int interval = 500, int flashTimes = 1)
        {
            if (source == null)
                return;

            MapUtilities.FlashFeature(source, interval, flashTimes);
        }

        /// <summary>
        ///     Highlights the specified feature using the color specified by the ArcFM Properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IElement" /> representing the graphic element that was highlighted.
        /// </returns>
        public static IElement Highlight(this IFeature source)
        {
            if (source == null)
                return null;

            return MapUtilities.HighlightFeature(source);
        }

        /// <summary>
        ///     Pans the map to the feature.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Pan(this IFeature source)
        {
            if (source == null)
                return;

            MapUtilities.PanFeature(source);
        }

        /// <summary>
        ///     Selects the specified feature in the map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="map">The active map.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the feature was selected; otherwise <c>false</c>
        /// </returns>
        public static bool Select(this IFeature source, IMap map)
        {
            if (source == null || map == null)
                return false;

            IFeatureLayer layer = map.GetFeatureLayer((IFeatureClass) source.Class);
            if (layer == null) return false;

            map.SelectFeature(layer, source);
            return true;
        }

        /// <summary>
        ///     Zooms the map to the specified feature.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Zoom(this IFeature source)
        {
            if (source == null)
                return;

            MapUtilities.ZoomFeature(source);
        }
#endif

        #endregion
    }
}