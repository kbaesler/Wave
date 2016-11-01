using System;
using System.Collections.Generic;
using System.Globalization;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Geodatabase.IField" /> interface.
    /// </summary>
    public static class FieldExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates an <see cref="IDictionary{TKey, TValue}" /> from an <see cref="IFields" />
        /// </summary>
        /// <param name="source">An <see cref="IFields" /> to create an <see cref="IDictionary{TKey, TValue}" /> from.</param>
        /// <returns>
        ///     An <see cref="IDictionary{TKey, TValue}" /> that contains the fields from the input source.
        /// </returns>
        public static IDictionary<string, int> ToDictionary(this IFields source)
        {
            return source.ToDictionary(field => field != null);
        }

        /// <summary>
        ///     Creates an <see cref="IDictionary{TKey, TValue}" /> from an <see cref="IFields" />
        /// </summary>
        /// <param name="source">An <see cref="IFields" /> to create an <see cref="IDictionary{TKey, TValue}" /> from.</param>
        /// <param name="predicate">The function used to determine if the field should be added to the dictionary.</param>
        /// <returns>
        ///     An <see cref="IDictionary{TKey, TValue}" /> that contains the fields from the input source.
        /// </returns>
        public static IDictionary<string, int> ToDictionary(this IFields source, Predicate<IField> predicate)
        {
            IDictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Create(CultureInfo.CurrentCulture, true));

            if (source != null)
            {
                for (int i = 0; i < source.FieldCount; i++)
                {
                    if (predicate(source.Field[i]))
                        dictionary.Add(source.Field[i].Name, i);
                }
            }

            return dictionary;
        }

        #endregion
    }
}