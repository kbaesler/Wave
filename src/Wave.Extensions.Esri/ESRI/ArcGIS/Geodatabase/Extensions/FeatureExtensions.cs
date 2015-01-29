using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IFeature" /> interface.
    /// </summary>
    public static class FeatureExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the difference in shape between the original and existing shape.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IGeometry" /> representing the difference in the shape; otherwise <c>null</c>
        /// </returns>
        public static IGeometry GetShapeDifference(this IFeature source)
        {
            IFeatureChanges featureChanges = (IFeatureChanges) source;
            if (featureChanges.ShapeChanged && featureChanges.OriginalShape != null)
            {
                ITopologicalOperator topologicalOperator = (ITopologicalOperator) source.ShapeCopy;
                return topologicalOperator.Difference(featureChanges.OriginalShape);
            }

            return null;
        }

        /// <summary>
        /// Updates the minimum display extent to reflect the changes to the feature to provide visual feedback.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="display">The display.</param>
        /// <param name="featureRenderer">The feature renderer.</param>
        public static void Invalidate(this IFeature source, IScreenDisplay display, IFeatureRenderer featureRenderer)
        {
            source.Invalidate(display, featureRenderer, esriScreenCache.esriAllScreenCaches);
        }

        /// <summary>
        ///     Updates the minimum display extent to reflect the changes to the feature to provide visual feedback.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="display">The display.</param>
        /// <param name="featureRenderer">The feature renderer.</param>
        /// <param name="screenCache">The screen cache.</param>
        public static void Invalidate(this IFeature source, IScreenDisplay display, IFeatureRenderer featureRenderer, esriScreenCache screenCache)
        {
            if (display == null || source == null || featureRenderer == null)
                return;

            ISymbol symbol = featureRenderer.SymbolByFeature[source];

            IInvalidArea3 invalidArea = new InvalidAreaClass();
            invalidArea.Display = display;
            invalidArea.AddFeature(source, symbol);
            invalidArea.Invalidate((short) screenCache);
        }

        #endregion
    }
}