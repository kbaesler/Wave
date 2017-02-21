using ESRI.ArcGIS.Carto;

using Miner.Framework;

namespace ESRI.ArcGIS.Geometry
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGeometry" /> interface.
    /// </summary>
    public static class GeometryExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Highlights the specified geometry using the color specified by the ArcFM Properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IElement" /> representing the graphic element that was highlighted.
        /// </returns>
        public static IElement Highlight(this IGeometry source)
        {
            return MapUtilities.HighlightFeature(source);
        }

        #endregion
    }
}