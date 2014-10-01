using ESRI.ArcGIS.ADF;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for managing the lifetime of COM objects.
    /// </summary>
    public static class ComReleaserExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Manages the lifetime of any COM object. The method will deterministically
        ///     release the object during the dispose process.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="values">The COM object to manage.</param>
        public static void ManageLifetime(this ComReleaser source, params object[] values)
        {
            foreach (var o in values)
            {
                source.ManageLifetime(o);
            }
        }

        #endregion
    }
}