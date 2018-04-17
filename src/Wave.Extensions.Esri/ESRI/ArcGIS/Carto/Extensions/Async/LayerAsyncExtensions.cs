using System.Collections.Generic;
using System.ComponentModel;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace ESRI.ArcGIS.Carto
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IEnumLayer" /> enumeration.
    /// </summary>
    public static class LayerAsyncExtensions
    {
        #region Public Methods

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
        /// <returns>
        ///     Returns a <see cref="IEnumerable{IFeatureIdentifyObj}" /> representing the features that have been identified.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">geometry</exception>
        /// <remarks>
        ///     On a FeatureIdentifyObject, you can do a QI to the IIdentifyObj interface to get more information about the
        ///     identified feature. The IIdentifyvsObj interface returns the window handle, layer, and name of the feature; it has
        ///     methods to flash the
        ///     feature in the display and to display a context menu at the Identify location.
        /// </remarks>
        public static IEnumerable<IFeatureIdentifyObj> IdentifyAsync(this IFeatureLayer source, IGeometry geometry, ITrackCancel trackCancel)
        {
            return Task.Wait(() => source.Identify(geometry, trackCancel));
        }

        #endregion
    }
}