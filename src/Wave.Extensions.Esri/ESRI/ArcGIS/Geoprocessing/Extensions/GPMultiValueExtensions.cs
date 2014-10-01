using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGPMultiValue" /> interface.
    /// </summary>
    public static class GPMultiValueExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IEnumerable{IGPValue}" /> from an <see cref="IGPMultiValue" />
        /// </summary>
        /// <param name="source">An <see cref="IGPMultiValue" /> to create an <see cref="IEnumerable{IGPValue}" /> from.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{IGPValue}" /> that contains the fields from the input source.
        /// </returns>
        public static IEnumerable<IGPValue> AsEnumerable(this IGPMultiValue source)
        {
            if (source != null)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    yield return source.Value[i];
                }
            }
        }

        #endregion
    }
}