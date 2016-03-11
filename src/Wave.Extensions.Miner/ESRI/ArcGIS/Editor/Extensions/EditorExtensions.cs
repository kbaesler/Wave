using System;

using Miner.Interop;

namespace ESRI.ArcGIS.Editor
{
    /// <summary>
    ///     Provides extension methods forthe <see cref="IEditor" /> extension.
    /// </summary>
    public static class EditorExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Encapsulates the <paramref name="operation" /> in the necessary start and stop operation constructs.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="mode">The mode used for the ArcFM Auto Updaters.</param>
        /// <param name="operation">The delegate that performs the operation.</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> when the operation completes.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">source;An edit operation is already started.</exception>
        public static bool PerformOperation(this IEditor source, string menuText, mmAutoUpdaterMode mode, Func<bool> operation)
        {
            using (new AutoUpdaterModeReverter(mode))
            {
                return source.PerformOperation(menuText, operation);
            }
        }

        #endregion
    }
}