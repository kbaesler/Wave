using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
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
        ///     Flashes to the specified feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The application reference.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        public static bool Flash(this IFeature source, IApplication hook)
        {
            return source.DoAction(hook, esriHookActions.esriHookActionsFlash);
        }

        /// <summary>
        ///     Flashes to the specified features.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The application reference.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        public static bool Flash(this IEnumerable<IFeature> source, IApplication hook)
        {
            return source.DoAction(hook, esriHookActions.esriHookActionsFlash);
        }

        /// <summary>
        ///     Gets the difference in shape between the original and existing shape.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IGeometry" /> representing the difference in the shape; otherwise <c>null</c>
        /// </returns>
        public static IGeometry GetDifference(this IFeature source)
        {
            if (source == null) return null;

            IFeatureChanges featureChanges = (IFeatureChanges) source;
            if (featureChanges.ShapeChanged && featureChanges.OriginalShape != null)
            {
                ITopologicalOperator topologicalOperator = (ITopologicalOperator) source.ShapeCopy;
                return topologicalOperator.Difference(featureChanges.OriginalShape);
            }

            return null;
        }

        /// <summary>
        ///     Updates the minimum display extent to reflect the changes to the feature to provide visual feedback.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="display">The display.</param>
        /// <param name="featureRenderer">The feature renderer.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     display
        ///     or
        ///     featureRenderer
        /// </exception>
        public static void Invalidate(this IFeature source, IScreenDisplay display, IFeatureRenderer featureRenderer)
        {
            if (source == null) return;
            if (display == null) throw new ArgumentNullException("display");
            if (featureRenderer == null) throw new ArgumentNullException("featureRenderer");

            source.Invalidate(display, featureRenderer, esriScreenCache.esriAllScreenCaches);
        }

        /// <summary>
        ///     Updates the minimum display extent to reflect the changes to the feature to provide visual feedback.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="display">The display.</param>
        /// <param name="featureRenderer">The feature renderer.</param>
        /// <param name="screenCache">The screen cache.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     display
        ///     or
        ///     featureRenderer
        /// </exception>
        public static void Invalidate(this IFeature source, IScreenDisplay display, IFeatureRenderer featureRenderer, esriScreenCache screenCache)
        {
            if (source == null) return;
            if (display == null) throw new ArgumentNullException("display");
            if (featureRenderer == null) throw new ArgumentNullException("featureRenderer");

            ISymbol symbol = featureRenderer.SymbolByFeature[source];

            IInvalidArea3 invalidArea = new InvalidAreaClass();
            invalidArea.Display = display;
            invalidArea.AddFeature(source, symbol);
            invalidArea.Invalidate((short) screenCache);
        }

        /// <summary>
        ///     Pans to the specified feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The application reference.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        public static bool Pan(this IFeature source, IApplication hook)
        {
            return source.DoAction(hook, esriHookActions.esriHookActionsPan);
        }

        /// <summary>
        ///     Pans to the specified features.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The application reference.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        public static bool Pan(this IEnumerable<IFeature> source, IApplication hook)
        {
            return source.DoAction(hook, esriHookActions.esriHookActionsPan);
        }

        /// <summary>
        ///     Zooms to the specified feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The application reference.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        public static bool Zoom(this IFeature source, IApplication hook)
        {
            return source.DoAction(hook, esriHookActions.esriHookActionsZoom);
        }

        /// <summary>
        ///     Zooms to the specified features.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The application reference.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        public static bool Zoom(this IEnumerable<IFeature> source, IApplication hook)
        {
            return source.DoAction(hook, esriHookActions.esriHookActionsZoom);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Executes the action on the feature.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The hook.</param>
        /// <param name="action">The action.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        private static bool DoAction(this IFeature source, IApplication hook, esriHookActions action)
        {
            var helper = new HookHelperClass();
            helper.Hook = hook;

            if (!helper.ActionSupported[source, action])
                return false;

            helper.DoAction(source, action);
            return true;
        }

        /// <summary>
        ///     Executes the action on all of the features in the enumeration.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="hook">The hook.</param>
        /// <param name="action">The action.</param>
        /// <returns>Returns <see cref="bool" /> representing <c>true</c> when the action was successful.</returns>
        private static bool DoAction(this IEnumerable<IFeature> source, IApplication hook, esriHookActions action)
        {
            var helper = new HookHelperClass();
            helper.Hook = hook;

            IArray array = new ArrayClass();
            foreach (var unk in source)
                array.Add(unk);

            if (!helper.ActionSupportedOnMultiple[array, action])
                return false;

            helper.DoActionOnMultiple(array, action);
            return true;
        }

        #endregion
    }
}