namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extensions for dynamic types.
    /// </summary>
    public static class DynamicRowBuffersExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the row buffer to a <see cref="DynamicRowBuffer" />
        /// </summary>
        /// <param name="source">The dictionary.</param>
        /// <returns>
        ///     Returns a <see cref="DynamicRowBuffer" /> representing the row.
        /// </returns>
        public static dynamic ToDynamic(this IRowBuffer source)
        {
            return new DynamicRowBuffer(source);
        }

        #endregion
    }
}