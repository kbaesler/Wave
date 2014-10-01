using System;

using ESRI.ArcGIS.Geodatabase;

namespace ESRI.ArcGIS.Geoprocessing
{
    /// <summary>
    ///     Provides extension methods for the <see cref="IGPValue" /> interface.
    /// </summary>
    public static class GPValueExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Casts an object to the type of the given default value. If the object is null
        ///     or DBNull, the default value specified will be returned. If the object is
        ///     convertible to the type of the default value, the explicit conversion will
        ///     be performed.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="fallbackValue">The fallback value.</param>
        /// <returns>
        ///     The value of the object cast to the type of the default value.
        /// </returns>
        /// <exception cref="System.InvalidCastException">
        ///     Thrown if the type of the object
        ///     cannot be cast to the type of the default value.
        /// </exception>
        public static TValue Cast<TValue>(this IGPValue source, TValue fallbackValue)
        {
            string value = source.GetAsText();
            return TypeCast.Cast(value, fallbackValue);
        }

        #endregion
    }
}